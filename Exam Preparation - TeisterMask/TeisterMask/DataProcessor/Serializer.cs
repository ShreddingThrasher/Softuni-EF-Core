namespace TeisterMask.DataProcessor
{
    using System;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Xml.Serialization;

    using Data;
    using ExportDto;

    using Formatting = Newtonsoft.Json.Formatting;
    using Newtonsoft.Json;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Projects");
            XmlSerializer serializer = new XmlSerializer(typeof(ExportProjectWithTasksDto[]), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            using StringWriter writer = new StringWriter(sb);

            ExportProjectWithTasksDto[] dtos = context.Projects
                .Where(p => p.Tasks.Any())
                .Include(p => p.Tasks)
                .ToArray()
                .Select(p => new ExportProjectWithTasksDto()
                {
                    TasksCount = p.Tasks.Count,
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                    Tasks = p.Tasks
                                .Select(t => new ExportProjectTaskDto()
                                {
                                    Name = t.Name,
                                    Label = t.LabelType.ToString()
                                })
                                .OrderBy(t => t.Name)
                                .ToArray()
                })
                .OrderByDescending(p => p.TasksCount)
                .ThenBy(p => p.ProjectName)
                .ToArray();

            serializer.Serialize(writer, dtos, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            ExportMostBusiestEmployeeDto[] dtos = context.Employees
                .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                .Include(e => e.EmployeesTasks)
                .ThenInclude(et => et.Task)
                .ToArray()
                .Select(e => new ExportMostBusiestEmployeeDto()
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                                .Where(et => et.Task.OpenDate >= date)
                                .Select(et => et.Task)
                                .OrderByDescending(t => t.DueDate)
                                .ThenBy(t => t.Name)
                                .Select(t => new ExportEmployeeTaskDto()
                                {
                                    TaskName = t.Name,
                                    OpenDate = t.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                                    DueDate = t.DueDate.ToString("d", CultureInfo.InvariantCulture),
                                    LabelType = t.LabelType.ToString(),
                                    ExecutionType = t.ExecutionType.ToString()
                                })
                                .ToArray()
                })
                .OrderByDescending(e => e.Tasks.Length)
                .ThenBy(e => e.Username)
                .Take(10)
                .ToArray();

            return JsonConvert.SerializeObject(dtos, Formatting.Indented);
        }
    }
}