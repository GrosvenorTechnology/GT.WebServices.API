using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GT.WebServices.API.Application.Dtos;
using GT.WebServices.API.Domain;

namespace GT.WebServices.API.Services
{
   public interface IEmployeeDataService
   {
      IQueryable<Employee> EmployeesQuery(string serialNumber);
      Task<int> GetCurrentEmployeeCount(string serialNumber);
      Task<List<Guid>> GetCurrentEmployeeIds(string serialNumber);
      Task<List<Employee>> GetCurrentEmployees(string serialNumber);
      Task<EmployeeDto> GetEmployeeDto(Guid employeeId);
      Task Update(Employee employee);
   }
}