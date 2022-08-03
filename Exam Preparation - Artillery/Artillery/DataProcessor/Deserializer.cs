namespace Artillery.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;

    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage =
                "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Countries");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCountryDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);

            ImportCountryDto[] countryDtos = (ImportCountryDto[])serializer.Deserialize(reader);

            List<Country> countries = new List<Country>();

            foreach (var dto in countryDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Country country = new Country()
                {
                    CountryName = dto.CountryName,
                    ArmySize = dto.ArmySize
                };

                countries.Add(country);
                sb.AppendLine(
                    string.Format(SuccessfulImportCountry, country.CountryName, country.ArmySize));
            }

            context.Countries.AddRange(countries);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Manufacturers");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportManufacturerDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);

            ImportManufacturerDto[] manufacturerDtos = (ImportManufacturerDto[])serializer.Deserialize(reader);

            List<Manufacturer> manufacturers = new List<Manufacturer>();

            foreach (var dto in manufacturerDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if(manufacturers.Any(m => m.ManufacturerName == dto.ManufacturerName))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Manufacturer manufacturer = new Manufacturer()
                {
                    ManufacturerName = dto.ManufacturerName,
                    Founded = dto.Founded
                };

                manufacturers.Add(manufacturer);
                sb.AppendLine(
                    string.Format(SuccessfulImportManufacturer, dto.ManufacturerName, 
                        string.Join(", ", dto.Founded.Split(", ").TakeLast(2))));
            }

            context.Manufacturers.AddRange(manufacturers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Shells");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportShellDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);

            ImportShellDto[] shellDtos = (ImportShellDto[])serializer.Deserialize(reader);

            List<Shell> shells = new List<Shell>();

            foreach (var dto in shellDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Shell shell = new Shell()
                {
                    ShellWeight = dto.ShellWeight,
                    Caliber = dto.Caliber
                };

                shells.Add(shell);
                sb.AppendLine(
                    string.Format(SuccessfulImportShell, shell.Caliber, shell.ShellWeight));
            }

            context.Shells.AddRange(shells);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportGunDto[] gunDtos = JsonConvert.DeserializeObject<ImportGunDto[]>(jsonString);

            List<Gun> guns = new List<Gun>();

            foreach (var dto in gunDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                object gunType;
                bool isGunTypeValid = Enum.TryParse(typeof(GunType), dto.GunType, out gunType);

                if (!isGunTypeValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Gun gun = new Gun()
                {
                    ManufacturerId = dto.ManufacturerId,
                    GunWeight = dto.GunWeight,
                    BarrelLength = dto.BarrelLength,
                    NumberBuild = dto.NumberBuild,
                    Range = dto.Range,
                    GunType = (GunType)gunType,
                    ShellId = dto.ShellId
                };

                foreach (var countryGunDto in dto.Countries)
                {
                    gun.CountriesGuns.Add(
                        new CountryGun()
                        {
                            CountryId = countryGunDto.Id
                        });
                }

                guns.Add(gun);
                sb.AppendLine(
                    string.Format(SuccessfulImportGun, gun.GunType, gun.GunWeight, gun.BarrelLength));
            }

            context.Guns.AddRange(guns);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
