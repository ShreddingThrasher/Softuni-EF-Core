namespace SoftJail.DataProcessor
{

    using Data;
    using DataProcessor.ExportDto;
    using System;
    using System.Linq;

    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            ExportPrisonerWithCellsAndOfficers[] dtos = context
                .Prisoners
                .ToArray()
                .Where(p => ids.Contains(p.Id))
                .Select(p => new ExportPrisonerWithCellsAndOfficers()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                                    .Select(po => new ExportPrisonerOfficerDto()
                                    {
                                        OfficerName = po.Officer.FullName,
                                        Department = po.Officer.Department.Name
                                    })
                                    .OrderBy(o => o.OfficerName)
                                    .ToArray(),
                    TotalOfficerSalary = p.PrisonerOfficers
                                            .Select(po => po.Officer.Salary)
                                            .Sum()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            return JsonConvert.SerializeObject(dtos, Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            string[] names = prisonersNames.Split(',');

            ExportInboxForPrisonerDto[] dtos = context
                .Prisoners
                .Where(p => names.Contains(p.FullName))
                .Select(p => new ExportInboxForPrisonerDto()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd"),
                    EncryptedMessages = p.Mails
                        .Select(m => new ExportPrisonerMailDto()
                        {
                            Description = ReverseDescription(m.Description)
                        })
                        .ToArray()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Prisoners");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer serializer = new XmlSerializer(typeof(ExportInboxForPrisonerDto[]), xmlRoot);

            StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, dtos, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ReverseDescription(IEnumerable<char> input)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in input.Reverse())
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}