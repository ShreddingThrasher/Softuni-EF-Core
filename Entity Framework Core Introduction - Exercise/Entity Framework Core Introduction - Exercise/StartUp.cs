using System;
using SoftUni.Data;
using System.Linq;
using System.Text;
using SoftUni.Models;
using Microsoft.EntityFrameworkCore;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using SoftUniContext context = new SoftUniContext();

            string output = RemoveTown(context);

            Console.WriteLine(output);
        }

        //03.Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            
            var employees = context.Employees
                .OrderBy(e => e.EmployeeId)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //04.Employees With Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //05. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //06. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Address address = new Address();
            address.AddressText = "Vitoshka 15";
            address.TownId = 4;

            context.Addresses.Add(address);

            var employee = context.Employees
                .Where(e => e.LastName == "Nakov")
                .FirstOrDefault();

            employee.Address = address;

            context.SaveChanges();

            var employeesAddresses = context.Employees
                .OrderByDescending(e => e.Address.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText);

            foreach (var text in employeesAddresses)
            {
                sb.AppendLine(text);
            }

            return sb.ToString().TrimEnd();
        }

        //07. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesWithProjects = context.Employees
                .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 &&
                                                          ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    AllProjects = e.EmployeesProjects
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                            EndDate = ep.Project.EndDate.HasValue ?
                                ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished"
                        })
                        .ToArray()
                })
                .ToArray();

            foreach (var e in employeesWithProjects)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                foreach (var p in e.AllProjects)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //08. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addresses = context.Addresses
                .Select(a => new
                {
                    a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = context.Employees
                        .Where(e => e.AddressId == a.AddressId)
                        .Count()
                })
                .OrderByDescending(a => a.EmployeesCount)
                .ThenBy(a => a.TownName)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToArray();

            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeesCount} employees");
            }

            return sb.ToString().TrimEnd();
        }


        //09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                        })
                        .ToArray()
                })
                .FirstOrDefault();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var p in employee.Projects.OrderBy(p => p.ProjectName))
            {
                sb.AppendLine(p.ProjectName);
            }

            return sb.ToString().TrimEnd();
        }

        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context.Departments
                                .Include(d => d.Employees)
                                .ThenInclude(d => d.Manager)
                                .Where(d => d.Employees.Count() > 5)
                                .OrderBy(d => d.Employees.Count())
                                .ThenBy(d => d.Name)
                                .Select(d => new
                                {
                                    d.Name,
                                    ManagerFirstName = d.Manager.FirstName,
                                    ManagerLastName = d.Manager.LastName,
                                    Employees = d.Employees
                                })
                                .ToArray();


            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.ManagerFirstName} {d.ManagerLastName}");

                foreach (var e in d.Employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }


        //11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var latestProjects = context.Projects
                                    .OrderByDescending(p => p.StartDate)
                                    .Take(10)
                                    .Select(p => new
                                    {
                                        p.Name,
                                        p.Description,
                                        StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                                    })
                                    .OrderBy(p => p.Name)
                                    .ToArray();

            foreach (var p in latestProjects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }

            return sb.ToString().TrimEnd();
        }


        //12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                .Where(e => e.Department.Name == "Engineering" ||
                                            e.Department.Name == "Tool Design" ||
                                            e.Department.Name == "Marketing" ||
                                            e.Department.Name == "Information Services")
                                .ToArray();

            foreach (var e in employees)
            {
                e.Salary *= (decimal)1.12;
            }

            context.SaveChanges();

            var employeesWithIncreasedSalaries = context.Employees
                                                    .Where(e => e.Department.Name == "Engineering" ||
                                                                e.Department.Name == "Tool Design" ||
                                                                e.Department.Name == "Marketing" ||
                                                                e.Department.Name == "Information Services")
                                                    .Select(e => new
                                                    {
                                                        e.FirstName,
                                                        e.LastName,
                                                        e.Salary
                                                    })
                                                    .OrderBy(e => e.FirstName)
                                                    .ThenBy(e => e.LastName)
                                                    .ToArray();

            foreach (var e in employeesWithIncreasedSalaries.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }


        //13. Find Employees by First Name Starting with Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                .ToArray()
                                .Where(e => e.FirstName.StartsWith("Sa", StringComparison.OrdinalIgnoreCase))
                                .Select(e => new
                                {
                                    e.FirstName,
                                    e.LastName,
                                    e.JobTitle,
                                    e.Salary
                                })
                                .OrderBy(e => e.FirstName)
                                .ThenBy(e => e.LastName);


            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectsToRemove = context.EmployeesProjects
                                        .Where(p => p.ProjectId == 2);

            context.EmployeesProjects.RemoveRange(projectsToRemove);
            context.SaveChanges();

            var project = context.Projects.Find(2);

            context.Projects.Remove(project);
            context.SaveChanges();

            var projectsToPrint = context.Projects
                                    .Take(10)
                                    .Select(p => p.Name)
                                    .ToArray();

            return string.Join(Environment.NewLine, projectsToPrint);
        }

        //15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            var employeesToChange = context.Employees
                                        .Where(e => e.Address.Town.Name == "Seattle");

            foreach (var e in employeesToChange)
            {
                e.AddressId = null;
            }

            context.SaveChanges();


            var addressesToDelete = context.Addresses
                                        .Where(a => a.Town.Name == "Seattle");
            int deletedCount = addressesToDelete.Count();
            context.Addresses.RemoveRange(addressesToDelete);
            context.SaveChanges();

            var townToDelete = context.Towns.FirstOrDefault(t => t.Name == "Seattle");
            context.Towns.Remove(townToDelete);
            context.SaveChanges();

            return $"{deletedCount} addresses in Seattle were deleted";
        }
    }
}
