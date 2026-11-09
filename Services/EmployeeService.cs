using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;
        PasswordHasher<Loginusers> _passwordHasher;

        public EmployeeService(AppDbContext db, PasswordHasher<Loginusers> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        // CREATE EMPLOYEE
        public async Task<EmployeewithIDDTO> CreateEmployeeAsync(EmployeeDTO employee)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // Check duplicate employee code
                bool employeeCodeExists = await _db.Employees
                    .AnyAsync(e => e.EmployeeCode == employee.EmployeeCode);

                if (employeeCodeExists)
                {
                    throw new Exception("Employee code already exists.");
                }

                // Check duplicate employee email
                bool emailExists = await _db.Employees
                    .AnyAsync(e => e.Email == employee.Email);

                if (emailExists)
                {
                    throw new Exception("Email already exists.");
                }

                // Check duplicate login username
                bool usernameExists = await _db.Loginuser
                    .AnyAsync(u => u.username == employee.Email);

                if (usernameExists)
                {
                    throw new Exception("Login account already exists for this email.");
                }

                if (!employee.ManagerId.HasValue)
                {
                    throw new Exception("Manager id is required.");
                }

                var resolvedManagerId = employee.ManagerId.Value;

                var validManager = await _db.Employees
                    .Include(e => e.loginuser)
                    .FirstOrDefaultAsync(e =>
                        e.EmployeeId == resolvedManagerId &&
                        e.IsActive);

                if (validManager == null)
                {
                    throw new Exception("Manager not found or inactive.");
                }

                if (validManager.loginuser == null ||
                    !validManager.loginuser.role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Invalid manager id. Employees must report to a manager.");
                }

                var loginUser = new Loginusers
                {
                    username = employee.Email,
                    role = "Employee"
                };

                // Hash password
                loginUser.passwordhash = _passwordHasher.HashPassword(
                    loginUser,
                    employee.Password
                );

                _db.Loginuser.Add(loginUser);

                await _db.SaveChangesAsync();



                var emp = new Employee
                {
                    EmployeeId = loginUser.employeeid,
                    EmployeeCode = employee.EmployeeCode,
                    Name = employee.Name,
                    Email = employee.Email,
                    Department = employee.Department,
                    JoiningDate = employee.JoiningDate,
                    IsActive = true,
                    ManagerId = resolvedManagerId,
                    loginuser=loginUser
                };

                _db.Employees.Add(emp);

                await _db.SaveChangesAsync();


                var leaveTypes = await _db.Leavetypes
                    .ToListAsync();

                foreach (var leaveType in leaveTypes)
                {
                    var leaveBalance = new Leavebalance
                    {
                        employeeid = emp.EmployeeId,
                        leavetypeid = leaveType.leavetypeid,
                        totaldays = leaveType.maximumdays,
                        useddays = 0
                    };

                    _db.Leavebalances.Add(leaveBalance);
                }

                await _db.SaveChangesAsync();
                

                // =====================================================
                // 4. COMMIT EVERYTHING
                // =====================================================

                await transaction.CommitAsync();

                return new EmployeewithIDDTO
                {
                    EmployeeId = emp.EmployeeId,
                    EmployeeCode = emp.EmployeeCode,
                    Name = emp.Name,
                    Email = emp.Email,
                    Department = emp.Department,
                    JoiningDate = emp.JoiningDate,
                    IsActive = emp.IsActive,
                    ManagerId = emp.ManagerId,
                    ManagerName = validManager.Name,
                    role = loginUser.role

                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

        public async Task<bool> ChangeStatusByManagerAsync(
            int managerId,
            int employeeId,
            bool isActive)
        {
            var manager = await _db.Employees
                .Include(e => e.loginuser)
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == managerId &&
                    e.IsActive);

            if (manager == null ||
                manager.loginuser == null ||
                !manager.loginuser.role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid manager.");
            }

            var employee = await _db.Employees
                .Include(e => e.loginuser)
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == employeeId);

            if (employee == null)
            {
                return false;
            }

            if (employee.loginuser == null ||
                !employee.loginuser.role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Managers can change status only for employees.");
            }

            if (employee.ManagerId != managerId)
            {
                throw new ArgumentException("You can change status only for your direct report employees.");
            }

            employee.IsActive = isActive;
            await _db.SaveChangesAsync();
            return true;
        }


        // GET EMPLOYEES FOR MANAGER
        public async Task<List<EmployeewithIDDTO>> GetEmployeesByManagerAsync(int managerId)
        {
            return await _db.Employees
                .Where(e => e.ManagerId == managerId && e.loginuser!.role == "Employee")
                .Select(e => new EmployeewithIDDTO
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    Name = e.Name,
                    Email = e.Email,
                    Department = e.Department,
                    JoiningDate = e.JoiningDate,
                    IsActive = e.IsActive,
                    ManagerId = e.ManagerId,
                    ManagerName = e.Manager != null ? e.Manager.Name : null,
                    role = e.loginuser!.role
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
                    ManagerId = e.ManagerId,
                    ManagerName = e.Manager != null ? e.Manager.Name : null,
                    role = e.loginuser!.role
                })
                .FirstOrDefaultAsync();
        }


        // UPDATE EMPLOYEE
        public async Task<object?> UpdateAsync(
            EmployeewithIDDTO employee)
        {
            var emp = await _db.Employees
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == employee.EmployeeId);

            if (emp == null)
            {
                return null;
            }
            if(emp.IsActive==true && employee.IsActive == false)
            {
                return "Employee can't be deactivated with this put method";
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
            loginuser!.username = employee.Email;
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
                ManagerId = emp.ManagerId,
                ManagerName = await _db.Employees
                    .Where(e => e.EmployeeId == emp.ManagerId)
                    .Select(e => e.Name)
                    .FirstOrDefaultAsync(),
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
                IsActive = true
               
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