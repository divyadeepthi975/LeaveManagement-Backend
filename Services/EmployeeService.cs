using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db ;
        public EmployeeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Employee> AddAsync(EmployeeDTO employee)
        {
            bool exists = await _db.Employees.AnyAsync(e => e.EmployeeCode == employee.EmployeeCode);

            if (exists)
            {
                throw new Exception("Employee code already exists.");
            }

            Employee emp = new Employee();
            emp.EmployeeCode = employee.EmployeeCode;
            emp.Name = employee.Name;
            emp.Department = employee.Department;
            emp.Email = employee.Email;
            emp.JoiningDate = employee.JoiningDate;

            _db.Employees.Add(emp);
            await _db.SaveChangesAsync();
            return emp;
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

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _db.Employees.ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _db.Employees.FindAsync(id);
        }

        public async Task<Employee?> UpdateAsync(Employee employee)
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
