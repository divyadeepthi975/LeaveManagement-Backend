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
            bool exists = await _db.Employees
                .AnyAsync(e => e.EmployeeCode == employee.EmployeeCode);

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

            var leavetypes = await _db.Leavetypes.Where(l=>l.IsActive==true).ToListAsync();

            foreach (var leavetype in leavetypes)
            {
                Leavebalance leave = new Leavebalance();

                leave.employeeid = emp.EmployeeId;
                leave.leavetypeid = leavetype.leavetypeid;
                leave.totaldays = leavetype.maximumdays;
                leave.useddays = 0;

                _db.Leavebalances.Add(leave);
            }

            await _db.SaveChangesAsync();

            return emp;
        }

        public async Task<string> DeleteAsync(int id)
        {
            var emp=await _db.Employees.FirstOrDefaultAsync(e=>e.EmployeeId==id);
            if (emp == null)
            {
                return "Employee with this ID not found" ;
            }
            if (emp.IsActive == false)
            {
                return "Employee is inactive or left";
            }
            emp.IsActive=false;
            await _db.SaveChangesAsync();
            return "Employee deleted successfully";
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _db.Employees.ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            Employee? emp= await _db.Employees.FindAsync(id);
            if (emp==null) {
                return null;
            }
            return emp;
        }


        public async Task<Employee?> UpdateAsync(EmployeewithIDDTO employee)
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
            emp.IsActive = employee.IsActive;
            await _db.SaveChangesAsync();
            return emp;
        }
        public async Task<Employee> AddWithIdAsync(EmployeeDTO employeeDTO,int employeeId)
        {
            bool codeExists = await _db.Employees
                .AnyAsync(e =>
                    e.EmployeeCode == employeeDTO.EmployeeCode);

            if (codeExists)
            {
                throw new Exception("Employee code already exists.");
            }

            bool emailExists = await _db.Employees
                .AnyAsync(e =>
                    e.Email == employeeDTO.Email);

            if (emailExists)
            {
                throw new Exception("Email already exists.");
            }

            var employee = new Employee
            {
                EmployeeId = employeeId,
                EmployeeCode = employeeDTO.EmployeeCode,
                Name = employeeDTO.Name,
                Email = employeeDTO.Email,
                Department = employeeDTO.Department,
                JoiningDate = employeeDTO.JoiningDate,
                IsActive = true
            };

            _db.Employees.Add(employee);

            await _db.SaveChangesAsync();

            var leaveTypes = await _db.Leavetypes.ToListAsync();

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
