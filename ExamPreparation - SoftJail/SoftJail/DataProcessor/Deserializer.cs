namespace SoftJail.DataProcessor
{

    using Data;
    using Data.Models;
    using DataProcessor.ImportDto;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Serialization;
    using System.IO;
    using SoftJail.Data.Models.Enums;
    using System.Text.RegularExpressions;
    using SoftJail.Common;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";
        private const string SuccessfullyImportedDepartment = "Imported {0} with {1} cells";
        private const string SuccessfullyImportedPrisoner = "Imported {0} {1} years old";
        private const string SuccessfullyImportedOfficer = "Imported {0} ({1} prisoners)";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportDepartmentWithCellsDto[] dtos = JsonConvert
                .DeserializeObject<ImportDepartmentWithCellsDto[]>(jsonString);

            List<Department> departments = new List<Department>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if(dto.Cells.Length == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Department department = new Department()
                {
                    Name = dto.Name
                };

                bool areCellsValid = true;

                foreach (var cellDto in dto.Cells)
                {
                    if (!IsValid(cellDto))
                    {
                        areCellsValid = false;
                        break;
                    }

                    department.Cells.Add(new Cell()
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    });
                }

                if (!areCellsValid || department.Cells.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                departments.Add(department);

                sb.AppendLine(string.Format(SuccessfullyImportedDepartment,
                    department.Name, department.Cells.Count));
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportPrisonerWithMailsDto[] dtos = JsonConvert
                .DeserializeObject<ImportPrisonerWithMailsDto[]>(jsonString);

            List<Prisoner> prisoners = new List<Prisoner>();

            foreach (var prisonerDto in dtos)
            {

                if (!IsValid(prisonerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isIncarcerationDateValid = DateTime.TryParseExact(
                    prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime incarcerationDate);

                if (!isIncarcerationDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? releaseDate = string.IsNullOrEmpty(prisonerDto.ReleaseDate) ?
                    null : (DateTime?)DateTime.ParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture);

                Prisoner prisoner = new Prisoner()
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId
                };

                bool areMailsValid = true;

                foreach (var mailDto in prisonerDto.Mails)
                {
                    if (!IsValid(mailDto))
                    {
                        areMailsValid = false;
                        break;
                    }

                    prisoner.Mails.Add(new Mail()
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    });
                }

                if (!areMailsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                prisoners.Add(prisoner);

                sb.AppendLine(string.Format(
                    SuccessfullyImportedPrisoner, prisoner.FullName, prisoner.Age));
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Officers");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportOfficerDto[]), xmlRoot);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            using StringReader reader = new StringReader(xmlString);

            ImportOfficerDto[] dtos = (ImportOfficerDto[])serializer.Deserialize(reader);

            List<Officer> officers = new List<Officer>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                object position;
                object weapon;

                bool isPositionValid = Enum.TryParse(typeof(Position), dto.Position,
                    out position);

                bool isWeaponValid = Enum.TryParse(typeof(Weapon), dto.Weapon,
                    out weapon);

                if (!isPositionValid || !isWeaponValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Officer officer = new Officer()
                {
                    FullName = dto.Name,
                    Salary = dto.Money,
                    Position = (Position)position,
                    Weapon = (Weapon)weapon,
                    DepartmentId = dto.DepartmentId,
                };

                foreach (var prisonerId in dto.Prisoners)
                {
                    officer.OfficerPrisoners.Add(new OfficerPrisoner()
                    {
                        PrisonerId = prisonerId.PrisonerId
                    });
                }

                officers.Add(officer);
                sb.AppendLine(string.Format(
                    SuccessfullyImportedOfficer, officer.FullName, officer.OfficerPrisoners.Count));
            }

            context.Officers.AddRange(officers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);

            return isValid;
        }

        private static bool IsValidTest(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);

            foreach (var result in validationResult)
            {
                Console.WriteLine(result.ErrorMessage);
            }
            return isValid;
        }
    }
}