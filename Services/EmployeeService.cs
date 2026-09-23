using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;

        public EmployeeService(AppDbContext db)
        {
            _db = db;
        }

        // CREATE EMPLOYEE
        public async Task<Employee> AddAsync(EmployeeDTO employee)
        {
            bool exists = await _db.Employees
                .AnyAsync(e =>
                    e.EmployeeCode == employee.EmployeeCode);

            if (exists)
            {
                throw new Exception(
                    "Employee code already exists.");
            }

            bool emailExists = await _db.Employees
                .AnyAsync(e =>
                    e.Email == employee.Email);

            if (emailExists)
            {
                throw new Exception(
                    "Email already exists.");
            }

            Employee emp = new Employee
            {
                EmployeeCode = employee.EmployeeCode,
                Name = employee.Name,
                Department = employee.Department,
                Email = employee.Email,
                JoiningDate = employee.JoiningDate,
                IsActive = true
                
            };

            _db.Employees.Add(emp);

            await _db.SaveChangesAsync();

            var leavetypes = await _db.Leavetypes
                .Where(l => l.IsActive == true)
                .ToListAsync();

            foreach (var leavetype in leavetypes)
            {
                Leavebalance leave = new Leavebalance
                {
                    employeeid = emp.EmployeeId,
                    leavetypeid = leavetype.leavetypeid,
                    totaldays = leavetype.maximumdays,
                    useddays = 0
                };

                _db.Leavebalances.Add(leave);
            }

            await _db.SaveChangesAsync();

            return emp;
        }


        // DELETE / SOFT DELETE EMPLOYEE
        public async Task<string> DeleteAsync(int id)
        {
            var emp = await _db.Employees
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == id);

            if (emp == null)
            {
                return "Employee with this ID not found";
            }

            if (emp.IsActive == false)
            {
                return "Employee is inactive or left";
            }

            emp.IsActive = false;

            await _db.SaveChangesAsync();

            return "Employee deleted successfully";
        }


        // GET ALL EMPLOYEES
        public async Task<IEnumerable<EmployeewithIDDTO>> GetAllAsync()
        {
            return await _db.Employees
                .Select(e => new EmployeewithIDDTO
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    Name = e.Name,
                    Email = e.Email,
                    Department = e.Department,
                    JoiningDate = e.JoiningDate,
                    IsActive = e.IsActive,
                    role = e.loginuser.role
                })
                .ToListAsync();
        }


        // GET EMPLOYEE BY ID
        public async Task<EmployeewithIDDTO?> GetByIdAsync(int id)
        {
            return await _db.Employees
                .Where(e => e.EmployeeId == id)
                .Select(e => new EmployeewithIDDTO
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    Name = e.Name,
                    Email = e.Email,
                    Department = e.Department,
                    JoiningDate = e.JoiningDate,
                    IsActive = e.IsActive,
                    role = e.loginuser.role
                })
                .FirstOrDefaultAsync();
        }


        // UPDATE EMPLOYEE
        public async Task<EmployeewithIDDTO?> UpdateAsync(
            EmployeewithIDDTO employee)
        {
            var emp = await _db.Employees
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == employee.EmployeeId);

            if (emp == null)
            {
                return null;
            }
            var loginuser=await _db.Loginuser.FirstOrDefaultAsync(l=>l.employeeid == employee.EmployeeId);

            // Don't update EmployeeId.
            // It is the primary key.

            emp.EmployeeCode = employee.EmployeeCode;
            emp.Name = employee.Name;
            emp.Email = employee.Email;
            emp.Department = employee.Department;
            emp.JoiningDate = employee.JoiningDate;
            emp.IsActive = employee.IsActive;
            loginuser.username = employee.Email;
            emp.loginuser = loginuser;

            await _db.SaveChangesAsync();

            return new EmployeewithIDDTO
            {
                EmployeeId = emp.EmployeeId,
                EmployeeCode = emp.EmployeeCode,
                Name = emp.Name,
                Email = emp.Email,
                Department = emp.Department,
                JoiningDate = emp.JoiningDate,
                IsActive = emp.IsActive,
                role = emp.loginuser.role
            };
        }


        // CREATE EMPLOYEE WITH EXISTING ID
        // Used when Loginusers is created first.
        public async Task<Employee> AddWithIdAsync(
            EmployeeDTO employeeDTO,
            int employeeId)
        {
            bool codeExists = await _db.Employees
                .AnyAsync(e =>
                    e.EmployeeCode == employeeDTO.EmployeeCode);

            if (codeExists)
            {
                throw new Exception(
                    "Employee code already exists.");
            }

            bool emailExists = await _db.Employees
                .AnyAsync(e =>
                    e.Email == employeeDTO.Email);

            if (emailExists)
            {
                throw new Exception(
                    "Email already exists.");
            }

            var employee = new Employee
            {
                EmployeeId = employeeId,
                EmployeeCode = employeeDTO.EmployeeCode,
                Name = employeeDTO.Name,
                Email = employeeDTO.Email,
                Department = employeeDTO.Department,
                JoiningDate = employeeDTO.JoiningDate,
                IsActive = true,
                loginuser = employeeDTO.loginuser
            };

            _db.Employees.Add(employee);

            await _db.SaveChangesAsync();

            var leaveTypes = await _db.Leavetypes
                .Where(l => l.IsActive == true)
                .ToListAsync();

            foreach (var leaveType in leaveTypes)
            {
                var leaveBalance = new Leavebalance
                {
                    employeeid = employeeId,
                    leavetypeid = leaveType.leavetypeid,
                    totaldays = leaveType.maximumdays,
                    useddays = 0
                };

                _db.Leavebalances.Add(leaveBalance);
            }

            await _db.SaveChangesAsync();

            return employee;
        }
    }
}