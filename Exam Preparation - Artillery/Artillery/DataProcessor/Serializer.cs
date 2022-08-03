
namespace Artillery.DataProcessor
{
    using Artillery.DataProcessor.ExportDto;
    using Artillery.Data;
    using Artillery.Data.Models.Enums;
    using System;
    using System.Linq;

    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Serialization;
    using System.IO;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            ExportShellWithGunsDto[] dtos = context
                .Shells
                .Where(s => s.ShellWeight > shellWeight)
                .Select(s => new ExportShellWithGunsDto()
                {
                    ShellWeight = s.ShellWeight,
                    Caliber = s.Caliber,
                    Guns = s.Guns
                        .Where(g => g.GunType == GunType.AntiAircraftGun)
                        .Select(g => new ExportShellGunDto()
                        {
                            GunType = g.GunType.ToString(),
                            GunWeight = g.GunWeight,
                            BarrelLength = g.BarrelLength,
                            Range = g.Range > 3000 ? "Long-range" : "Regular range"
                        })
                        .OrderByDescending(g => g.GunWeight)
                        .ToArray()
                })
                .OrderBy(s => s.ShellWeight)
                .ToArray();

            return JsonConvert.SerializeObject(dtos, Formatting.Indented);
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Guns");

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer serializer = new XmlSerializer(typeof(ExportGunWithCountriesDto[]), xmlRoot);

            ExportGunWithCountriesDto[] dtos = context
                .Guns
                .Where(g => g.Manufacturer.ManufacturerName == manufacturer)
                .Select(g => new ExportGunWithCountriesDto()
                {
                    Manufacturer = g.Manufacturer.ManufacturerName,
                    GunType = g.GunType.ToString(),
                    GunWeight = g.GunWeight,
                    BarrelLength = g.BarrelLength,
                    Range = g.Range,
                    Countries = g.CountriesGuns
                        .Where(gc => gc.Country.ArmySize > 4500000)
                        .Select(gc => gc.Country)
                        .Select(c => new ExportGunCountryDto()
                        {
                            Name = c.CountryName,
                            ArmySize = c.ArmySize
                        })
                        .OrderBy(c => c.ArmySize)
                        .ToArray()
                })
                .OrderBy(g => g.BarrelLength)
                .ToArray();

            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, dtos, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
