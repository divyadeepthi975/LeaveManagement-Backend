using LeaveManagement.Data;
using LeaveManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class EmployeeService 
    {
        private readonly AppDbContext _db ;
        public EmployeeService(AppDbContext db)
        {
            _db = db;
        }
        
        public async Task<Models.Entities.Employee> AddAsync(Models.Entities.Employee employee)
        {
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emp=await _db.Employees.FirstOrDefaultAsync(e=>e.EmployeeId==id);
            if (emp == null)
            {
                return false ;
            }
            _db.Employees.Remove(emp);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Models.Entities.Employee>> GetAllAsync()
        {
            return await _db.Employees.ToListAsync();
        }

        public async Task<Models.Entities.Employee?> GetByIdAsync(int id)
        {
            return await _db.Employees.FindAsync(id);
        }

        public async Task<Models.Entities.Employee?> UpdateAsync(Models.Entities.Employee employee)
        {
            var emp=await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);
            if (emp == null)
            {
                return null;
            }
            emp.EmployeeId= employee.EmployeeId;
            emp.EmployeeCode= employee.EmployeeCode;
            emp.Name= employee.Name;
            emp.Email= employee.Email;
            emp.Department= employee.Department;
            emp.JoiningDate= employee.JoiningDate;
            await _db.SaveChangesAsync();
            return emp;
        }
    }
}
