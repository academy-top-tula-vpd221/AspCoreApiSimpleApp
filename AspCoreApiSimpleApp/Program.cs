using System.Text.RegularExpressions;
using AspCoreApiSimpleApp;

List<Employee> employees = new()
{
    new() { Id = Guid.NewGuid().ToString(), Name = "Tommy", Age = 27 },
    new() { Id = Guid.NewGuid().ToString(), Name = "Bobby", Age = 31 },
    new() { Id = Guid.NewGuid().ToString(), Name = "Jimmy", Age = 43 },
};


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (context) =>
{
    var request = context.Request;
    var response = context.Response;

    var path = request.Path;
    var method = request.Method;

    string regexpGuid = @"^/api/empl/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";

    if(path == "/api/empl" && method == "GET" )
    {
        // senf all employees
        await GetAllEmployees(response);
    }
    else if(Regex.IsMatch(path, regexpGuid) && method == "GET")
    {
        string? id = path.Value?.Split("/")[3];
        await GetEmployee(id, response);
    }
    else if(path == "/api/empl" && method == "POST")
    {
        await InsertEmployee(request, response);
    }
    else if(path == "/api/empl" && method == "PUT")
    {
        await UpdateEmployee(request, response);
    }
    else if(Regex.IsMatch(path, regexpGuid) && method == "DELETE")
    {
        string? id = path.Value?.Split("/")[3];
        await DeleteEmployee(id, response);
    }

    else
    {
        response.ContentType = "text/html; charset = utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

async Task DeleteEmployee(string? id, HttpResponse response)
{
    Employee? employee = employees.FirstOrDefault(e => e.Id == id);

    if(employee is not null)
    {
        employees.Remove(employee);
        await response.WriteAsJsonAsync(employee);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { text = "Employee not found " });
    }
}

async Task UpdateEmployee(HttpRequest request, HttpResponse response)
{
    try
    {

        Employee? employee = await request.ReadFromJsonAsync<Employee>();

        if(employee is not null)
        {
            Employee? employeeUpdate = employees.FirstOrDefault(e => e.Id == employee.Id);

            if(employeeUpdate is not null)
            {
                employeeUpdate.Age = employee.Age;
                employeeUpdate.Name = employee.Name;
                await response.WriteAsJsonAsync(employeeUpdate);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { text = "Employee not found " });
            }
        }
        else
            throw new Exception("Incorrect data");
    }
    catch(Exception)
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { text = "Incorrect data" });
    }
}

async Task InsertEmployee(HttpRequest request, HttpResponse response)
{
    try
    {
        Employee? employee = await request.ReadFromJsonAsync<Employee>();

        if(employee is not null)
        {
            employee.Id = Guid.NewGuid().ToString();
            employees.Add(employee);
            await response.WriteAsJsonAsync(employee);
        }
        else
        {
            throw new Exception("Incorrect data");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { text = "Incorrect data" });
    }
}

async Task GetEmployee(string? id, HttpResponse response)
{
    Employee? employee = employees.FirstOrDefault(e => e.Id == id);

    if (employee is not null)
        await response.WriteAsJsonAsync(employee);
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { text = "Employee not found " });
    }
}

async Task GetAllEmployees(HttpResponse response)
{
    await response.WriteAsJsonAsync(employees);
}

app.Run();
