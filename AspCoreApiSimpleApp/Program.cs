using AspCoreApiSimpleApp;
using System.Text.RegularExpressions;
using System.Xml.Linq;

// начальные данные
List<Employee> employees = new List<Employee>
{
    new() { Id = Guid.NewGuid().ToString(), Name = "Tom", Age = 37 },
    new() { Id = Guid.NewGuid().ToString(), Name = "Bob", Age = 41 },
    new() { Id = Guid.NewGuid().ToString(), Name = "Sam", Age = 24 }
};

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/empls", () => employees);

app.MapGet("/api/empls/{id}", (string id) =>
{
    Employee? employee = employees.FirstOrDefault((e) => e.Id == id);

    if (employee != null)
        return Results.Json(employee);
    else
        return Results.NotFound(new { message = "Пользователь не найден" });
});

app.MapDelete("/api/empls/{id}", (string id) =>
{
    Employee? employee = employees.FirstOrDefault((e) => e.Id == id);
    if (employee != null)
    {
        employees.Remove(employee);
        return Results.Json(employee);
    }
    else
        return Results.NotFound(new { message = "Пользователь не найден" });
});

app.MapPost("/api/empls", (Employee employee) =>
{
    employee.Id = Guid.NewGuid().ToString();
    employees.Add(employee);
    return Results.Json(employee);
});

app.MapPut("/api/empls", (Employee employeeData) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == employeeData.Id);
    if (employee != null)
    {
        employee.Name = employeeData.Name;
        employee.Age = employeeData.Age;
        return Results.Json(employee);
    }
    else
        return Results.NotFound(new { message = "Пользователь не найден" });
});

app.Run();

/*
app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;
    var method = request.Method;

    string regexpGuid = @"^/api/empls/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
    if (path == "/api/empls" && method == "GET")
    {
        await GetAllEmployees(response);
    }
    else if (Regex.IsMatch(path, regexpGuid) && method == "GET")
    {
        string? id = path.Value?.Split("/")[3];
        await GetEmployee(id, response);
    }
    else if (path == "/api/empls" && method == "POST")
    {
        await CreateEmployee(response, request);
    }
    else if (path == "/api/empls" && method == "PUT")
    {
        await UpdateEmployee(response, request);
    }
    else if (Regex.IsMatch(path, regexpGuid) && method == "DELETE")
    {
        string? id = path.Value?.Split("/")[3];
        await DeleteEmployee(id, response);
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();

async Task GetAllEmployees(HttpResponse response)
{
    await response.WriteAsJsonAsync(employees);
}

async Task GetEmployee(string? id, HttpResponse response)
{
    Employee? employee = employees.FirstOrDefault((e) => e.Id == id);

    if (employee != null)
        await response.WriteAsJsonAsync(employee);
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
    }
}

async Task DeleteEmployee(string? id, HttpResponse response)
{
    Employee? employee = employees.FirstOrDefault((e) => e.Id == id);

    if (employee != null)
    {
        employees.Remove(employee);
        await response.WriteAsJsonAsync(employee);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
    }
}

async Task CreateEmployee(HttpResponse response, HttpRequest request)
{
    try
    {
        var employee = await request.ReadFromJsonAsync<Employee>();
        if (employee != null)
        {
            employee.Id = Guid.NewGuid().ToString();
            employees.Add(employee);
            await response.WriteAsJsonAsync(employee);
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
}

async Task UpdateEmployee(HttpResponse response, HttpRequest request)
{
    try
    {
        Employee? employeeData = await request.ReadFromJsonAsync<Employee>();
        if (employeeData != null)
        {
            var employee = employees.FirstOrDefault(e => e.Id == employeeData.Id);
            if (employee != null)
            {
                employee.Age = employeeData.Age;
                employee.Name = employeeData.Name;
                await response.WriteAsJsonAsync(employee);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
            }
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
}
*/