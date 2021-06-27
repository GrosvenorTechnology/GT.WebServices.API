using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GT.WebServices.API.Application.Dtos;
using Microsoft.Extensions.Logging;
using GT.WebServices.API.Domain;

namespace GT.WebServices.API.Services
{
   public class EmployeeDataService : IEmployeeDataService
   {
      private static List<Employee> DemoData;

      private readonly ILogger<EmployeeDataService> _logger;
      private readonly IMapper _mapper;

      static EmployeeDataService ()
      {
         LoadDemoData();
      }

      public EmployeeDataService(ILogger<EmployeeDataService> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }

      //public async Task<Guid> GetCompanyIdBySerialNumber(string serialNumber)
      //{
      //   return await (from e in DemoData
      //                 join dToAdg in _adsContext.DeviceToAdgs on serialNumber equals dToAdg.SerialNumber
      //                 join adg in _adsContext.AdvancedDistributionGroups on dToAdg.AdvancedDistributionGroupId equals adg.AdvancedDistributionGroupId
      //                 group adg by adg.CompanyId into t0
      //                 select t0.Key).SingleOrDefaultAsync();
      //}

      public IQueryable<Employee> EmployeesQuery(string serialNumber) =>
                  //Normalyy this would be a database query here, serial number is to allow you to implement some sor of device grouping
                  DemoData.AsQueryable();

      public async Task<EmployeeDto> GetEmployeeDto(Guid employeeId)
      {
         var employee = DemoData.AsQueryable().SingleOrDefault(x => x.EmployeeId == employeeId);
         var result = _mapper.Map<EmployeeDto>(employee);
         return result;
      }

      public async Task<List<Employee>> GetCurrentEmployees(string serialNumber)
      {
         return EmployeesQuery(serialNumber).Where(x => !x.IsDeleted).ToList();
      }

      public async Task<List<Guid>> GetCurrentEmployeeIds(string serialNumber)
      {
         return EmployeesQuery(serialNumber).Where(x => !x.IsDeleted).Select(x => x.EmployeeId).ToList();
      }

      public async Task<int> GetCurrentEmployeeCount(string serialNumber)
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
               Firstname = "John",
               Surname = "Smith",
               KeyPadId = "123456",
               BadgeCode = "6543658634",
               CreatedOn = DateTime.Parse("2021-01-01"),
               ModifiedOn = DateTime.Parse("2021-01-01"),
               Language = "en",
               Roles = "supervisor"
            },
            new Employee
            {
               EmployeeId = new Guid("7B9B0608-2D5F-4C2A-8EDE-8F6943A85782"),
               ExternalId = "222222",
               Firstname = "",
               Surname = "",
               KeyPadId = "",
               BadgeCode = "",
               CreatedOn = DateTime.Parse("2021-01-01"),
               ModifiedOn = DateTime.Parse("2021-01-03"),
               Language = "",
               Roles = "",
               IsDeleted = true
            }
         };
      }
   }
}
