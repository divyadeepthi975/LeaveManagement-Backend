using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<Loginusers> _passwordHasher;

        public AdminService(AppDbContext db)
        {
            _db = db;
            _passwordHasher = new PasswordHasher<Loginusers>();
        }

        private async Task<int> GetAnyAdminEmployeeIdAsync()
        {
            var adminEmployeeIds = await _db.Loginuser
                .Where(x => x.role == "Admin")
                .Select(x => x.employeeid)
                .ToListAsync();

            if (adminEmployeeIds.Count == 0)
            {
                throw new Exception("Admin user not found to assign manager hierarchy.");
            }

            if (adminEmployeeIds.Count > 1)
            {
                throw new Exception("Invalid setup: multiple admin users found.");
            }

            return adminEmployeeIds[0];
        }

        private async Task<int> ResolveRequiredManagerIdAsync(
            int? managerId,
            bool adminOnly)
        {
            if (!managerId.HasValue)
            {
                throw new Exception("Manager id is required.");
            }

            var manager = await _db.Employees
                .Include(e => e.loginuser)
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == managerId.Value &&
                    e.IsActive);

            if (manager == null)
            {
                throw new Exception("Manager not found or inactive.");
            }

            if (manager.loginuser == null)
            {
                throw new Exception("Invalid manager id.");
            }

            if (adminOnly)
            {
                if (!manager.loginuser.role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Invalid manager id. Manager users must report to an admin.");
                }

                return managerId.Value;
            }

            if (!manager.loginuser.role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid manager id. Employees must report to a manager.");
            }

            return managerId.Value;
        }

        public async Task<EmployeewithIDDTO> CreateUserAsync(
    CreateUserDTO dto,
    string role)
        {
            if (role != "Manager" && role != "Employee")
            {
                throw new Exception("Invalid role.");
            }

            
            var usernameExists = await _db.Loginuser
                .AnyAsync(x =>
                    x.username.ToLower() == dto.Email.ToLower());

            if (usernameExists)
            {
                throw new Exception("Username already exists.");
            }


            var employeeCodeExists = await _db.Employees
                .AnyAsync(x =>
                    x.EmployeeCode.ToLower() ==
                    dto.EmployeeCode.ToLower());

            if (employeeCodeExists)
            {
                throw new Exception("Employee code already exists.");
            }

            // Check duplicate employee email
            bool emailExists = await _db.Employees
                .AnyAsync(e => e.Email == dto.Email);

            if (emailExists)
            {
                throw new Exception("Email already exists.");
            }

            using var transaction =
                await _db.Database.BeginTransactionAsync();

            try
            {

                var loginUser = new Loginusers
                {
                    username = dto.Email.ToLower(),
                    role = role
                };

                loginUser.passwordhash =
                    _passwordHasher.HashPassword(
                        loginUser,
                        dto.Password);

                _db.Loginuser.Add(loginUser);

                await _db.SaveChangesAsync();

                var resolvedManagerId = role == "Manager"
                    ? await ResolveRequiredManagerIdAsync(dto.ManagerId, adminOnly: true)
                    : await ResolveRequiredManagerIdAsync(dto.ManagerId, adminOnly: false);

                var emp = new Employee
                {
                    EmployeeId = loginUser.employeeid,

                    EmployeeCode = dto.EmployeeCode,
                    Name = dto.Name,
                    Email = dto.Email,
                    Department = dto.Department,
                    JoiningDate = dto.JoiningDate,
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
                    var balance = new Leavebalance
                    {
                        employeeid = loginUser.employeeid,
                        leavetypeid = leaveType.leavetypeid,
                        totaldays = leaveType.maximumdays,
                        useddays = 0,
                        Employee=emp,
                        Leavetype=leaveType
                    };

                    _db.Leavebalances.Add(balance);
                }

                await _db.SaveChangesAsync();
               



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
                    ManagerName = await _db.Employees
                        .Where(e => e.EmployeeId == emp.ManagerId)
                        .Select(e => e.Name)
                        .FirstOrDefaultAsync(),
                    role = loginUser.role

                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> ChangeToManagerAsync(
            int employeeId)
        {
            var loginUser = await _db.Loginuser
                .FirstOrDefaultAsync(
                    x => x.employeeid == employeeId);

            if (loginUser == null)
            {
                return false;
            }

            var employee = await _db.Employees
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);

            if (employee == null)
            {
                return false;
            }

            if (!employee.IsActive)
            {
                throw new Exception("Inactive user role cannot be changed.");
            }

            if (loginUser.role.Equals(
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Admin role cannot be changed.");
            }


            // Already Manager
            if (loginUser.role.Equals(
                "Manager",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "User is already a Manager.");
            }


            loginUser.role = "Manager";

            employee.ManagerId = await GetAnyAdminEmployeeIdAsync();

            await _db.SaveChangesAsync();

            return true;
        }


        public async Task<bool> ChangeToEmployeeAsync(
            int employeeId,
            int managerId)
        {
            var loginUser = await _db.Loginuser
                .FirstOrDefaultAsync(
                    x => x.employeeid == employeeId);

            if (loginUser == null)
            {
                return false;
            }

            var employee = await _db.Employees
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);

            if (employee == null)
            {
                return false;
            }

            if (!employee.IsActive)
            {
                throw new Exception("Inactive user role cannot be changed.");
            }

            if (loginUser.role.Equals(
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Admin role cannot be changed.");
            }


            // Already Employee
            if (loginUser.role.Equals(
                "Employee",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "User is already an Employee.");
            }


            var resolvedManagerId = await ResolveRequiredManagerIdAsync(
                managerId,
                adminOnly: false);

            if (resolvedManagerId == employeeId)
            {
                throw new Exception("Employee cannot be their own manager.");
            }

            loginUser.role = "Employee";

            employee.ManagerId = resolvedManagerId;

            await _db.SaveChangesAsync();

            return true;
        }


        public async Task<bool> ChangeStatusAsync(
            int employeeId,
            ChangeStatusDTO dto)
        {
            var employee = await _db.Employees
                .FirstOrDefaultAsync(
                    x => x.EmployeeId == employeeId);

            if (employee == null)
            {
                return false;
            }


            var loginUser = await _db.Loginuser
                .FirstOrDefaultAsync(
                    x => x.employeeid == employeeId);

            if (loginUser == null)
            {
                return false;
            }


            // Admin status cannot be changed
            if (loginUser.role.Equals(
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Admin status cannot be changed.");
            }


            employee.IsActive = dto.IsActive;

            await _db.SaveChangesAsync();

            return true;
        }
        public async Task<List<GetAllEmployeesDTO>> GetAllUsersAsync()
        {
            var users = await _db.Employees
                .Select(employee => new GetAllEmployeesDTO
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeCode = employee.EmployeeCode,
                    Name = employee.Name,
                    Email = employee.Email,
                    Department = employee.Department,
                    JoiningDate = employee.JoiningDate,
                    Role = employee.loginuser!.role,
                    IsActive = employee.IsActive,
                    ManagerId = employee.ManagerId,
                    ManagerName = employee.Manager != null ? employee.Manager.Name : null
                })
                .ToListAsync();

            return users;
        }
 
public async Task<EmployeewithIDDTO?> GetEmployeeByIdAsync(
    int employeeId)
        {
            var employee = await _db.Employees
                .Include(e => e.loginuser)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == employeeId);

            if (employee == null)
            {
                return null;
            }

            return new EmployeewithIDDTO
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                JoiningDate = employee.JoiningDate,
                IsActive = employee.IsActive,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.Name,
                role = employee.loginuser!.role
            };
        }


        public async Task<EmployeewithIDDTO?> UpdateEmployeeAsync(
            EmployeewithIDDTO employee)
        {
            var existingEmployee = await _db.Employees
                .Include(e => e.loginuser)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == employee.EmployeeId);

            if (existingEmployee == null)
            {
                return null;
            }

            if (!existingEmployee.IsActive)
            {
                throw new ArgumentException(
                    "Employee is inactive.");
            }

            // Check Employee Code
            var employeeCodeExists = await _db.Employees
                .AnyAsync(e =>
                    e.EmployeeId != employee.EmployeeId &&
                    e.EmployeeCode == employee.EmployeeCode);

            if (employeeCodeExists)
            {
                throw new ArgumentException(
                    "Employee code already exists.");
            }

            // Check Email
            var emailExists = await _db.Employees
                .AnyAsync(e =>
                    e.EmployeeId != employee.EmployeeId &&
                    e.Email == employee.Email);

            if (emailExists)
            {
                throw new ArgumentException(
                    "Email already exists.");
            }

            existingEmployee.EmployeeCode =
                employee.EmployeeCode;

            existingEmployee.Name =
                employee.Name;

            existingEmployee.Email =
                employee.Email;

            existingEmployee.Department =
                employee.Department;

            existingEmployee.JoiningDate =
                employee.JoiningDate;

            if (existingEmployee.loginuser == null)
            {
                throw new ArgumentException("Employee login information is missing.");
            }

            if (!employee.ManagerId.HasValue)
            {
                throw new ArgumentException("Manager id is required.");
            }

            if (employee.ManagerId.Value == existingEmployee.EmployeeId)
            {
                throw new ArgumentException("Employee cannot be their own manager.");
            }

            if (existingEmployee.loginuser.role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                var adminEmployeeId = await GetAnyAdminEmployeeIdAsync();

                if (employee.ManagerId.Value != adminEmployeeId)
                {
                    throw new ArgumentException("Manager users must have Admin as manager.");
                }

                existingEmployee.ManagerId = adminEmployeeId;
            }
            else
            {
                existingEmployee.ManagerId = await ResolveRequiredManagerIdAsync(
                    employee.ManagerId,
                    adminOnly: false);
            }

            await _db.SaveChangesAsync();

            var managerName = await _db.Employees
                .Where(e => e.EmployeeId == existingEmployee.ManagerId)
                .Select(e => e.Name)
                .FirstOrDefaultAsync();

            return new EmployeewithIDDTO
            {
                EmployeeId = existingEmployee.EmployeeId,
                EmployeeCode = existingEmployee.EmployeeCode,
                Name = existingEmployee.Name,
                Email = existingEmployee.Email,
                Department = existingEmployee.Department,
                JoiningDate = existingEmployee.JoiningDate,
                IsActive = existingEmployee.IsActive,
                ManagerId = existingEmployee.ManagerId,
                ManagerName = managerName,
                role = existingEmployee.loginuser!.role
            };
        }




    }
}