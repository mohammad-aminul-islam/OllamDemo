using System.Text;
using System.Text.Json;

namespace OllamaAIDemo.Services;
public class EmployeeDataProvider
{
    public IEnumerable<Employee> GetEmployees()
    {
        try
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Services", "employee.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return Enumerable.Empty<Employee>();
            }
            string json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<IEnumerable<Employee>>(json);
            return data ?? Enumerable.Empty<Employee>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public string BuildContextPrompt(IEnumerable<Employee> employees, string query)
    {
        var avgSalary = employees.Average(e => e.salary);
        var totalEmployees = employees.Count();
        var countries = employees.Select(e => e.country).Distinct().ToList();
        var departments = employees.Select(e => e.department).Distinct().ToList();

        var context = new StringBuilder();
        context.AppendLine("===Dataset Summary===");
        context.AppendLine($"Total Employees: {totalEmployees}");
        context.AppendLine($"Average Salary: ${avgSalary:N2}");
        context.AppendLine($"Countries: {string.Join(", ", countries)}");
        context.AppendLine($"Departments: {string.Join(", ", departments)}");
        context.AppendLine();
        context.AppendLine("=== Employee Data===");
        foreach (var employee in employees)
        {
            context.AppendLine(@$"-{employee.name} , {employee.designation} , Salary: USD {employee.salary}, Department: {employee.department}, country:{employee.country}, yearOfExperience: {employee.yearsOfExperience}");
        }
        context.AppendLine();
        context.AppendLine($"User's Question on this employee data:{query}?");
        context.AppendLine();
 
        return context.ToString();
    }
}

public class Employee
{
    public string name { get; set; }
    public string designation { get; set; }
    public int salary { get; set; }
    public string department { get; set; }
    public string country { get; set; }
    public int yearsOfExperience { get; set; }
}
