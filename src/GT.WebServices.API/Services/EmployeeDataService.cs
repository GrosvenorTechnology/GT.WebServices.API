﻿using GT.WebServices.API.Application.Dtos;
using GT.WebServices.API.Domain;
using GT.WebServices.API.Application.Mapping;

namespace GT.WebServices.API.Services;

public class EmployeeDataService 
{
    private static List<Employee> DemoData;

    private readonly ILogger<EmployeeDataService> _logger;

    static EmployeeDataService()
    {
        LoadDemoData();
    }

    public EmployeeDataService(ILogger<EmployeeDataService> logger)
    {
        _logger = logger;
    }

    public IQueryable<Employee> EmployeesQuery(string serialNumber) =>
                //Normalyy this would be a database query here, serial number is to allow you to implement some sort of device grouping
                DemoData.AsQueryable();

    public EmployeeDto GetEmployeeDto(Guid employeeId)
    {
        var mapper = new AdsMapper();
        var employee = DemoData.AsQueryable().SingleOrDefault(x => x.EmployeeId == employeeId);
        var result = mapper.Map(employee);
        return result;
    }

    public List<Employee> GetCurrentEmployees(string serialNumber)
    {
        return EmployeesQuery(serialNumber).Where(x => !x.IsDeleted).ToList();
    }

    public List<Guid> GetCurrentEmployeeIds(string serialNumber)
    {
        return EmployeesQuery(serialNumber).Where(x => !x.IsDeleted).Select(x => x.EmployeeId).ToList();
    }

    public int GetCurrentEmployeeCount(string serialNumber)
    {
        return EmployeesQuery(serialNumber).Where(x => !x.IsDeleted).Count();
    }

    public Task Update(Employee employee)
    {
        employee.ModifiedOn = DateTime.UtcNow;

        var old = DemoData.First(x => x.EmployeeId == employee.EmployeeId);
        DemoData.Remove(old);
        DemoData.Add(employee);

        return Task.CompletedTask;
    }

    private static void LoadDemoData()
    {
        DemoData = new List<Employee>
     {
        new Employee
        {
           EmployeeId = new Guid("CCBB34B8-90DE-4976-93FA-C1B34F12A95C"),
           ExternalId = "1111111",
           Name = "John Smith",
           Firstname = "John",
           Surname = "Smith",
           KeyPadId = "1",
           BadgeCode = "1",
           CreatedOn = DateTime.Parse("2021-01-01"),
           ModifiedOn = DateTime.Parse("2022-02-01"),
           Language = "en",
           VerifyBy = "none",
           Roles = "supervisor"
        },
        new Employee
        {
           EmployeeId = new Guid("7B9B0608-2D5F-4C2A-8EDE-8F6943A85782"),
           ExternalId = "222222",
           Name = "",
           Firstname = "",
           Surname = "",
           KeyPadId = "",
           BadgeCode = "",
           CreatedOn = DateTime.Parse("2021-01-01"),
           ModifiedOn = DateTime.Parse("2021-01-03"),
           Language = "",
           Roles = "",
           VerifyBy = "",
           IsDeleted = true
        }
     };
    }
}
