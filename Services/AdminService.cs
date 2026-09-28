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

                var emp = new Employee
                {
                    EmployeeId = loginUser.employeeid,

                    EmployeeCode = dto.EmployeeCode,
                    Name = dto.Name,
                    Email = dto.Email,
                    Department = dto.Department,
                    JoiningDate = dto.JoiningDate,
                    IsActive = true,
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

            await _db.SaveChangesAsync();

            return true;
        }


        public async Task<bool> ChangeToEmployeeAsync(
            int employeeId)
        {
            var loginUser = await _db.Loginuser
                .FirstOrDefaultAsync(
                    x => x.employeeid == employeeId);

            if (loginUser == null)
            {
                return false;
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


            loginUser.role = "Employee";

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
                    IsActive = employee.IsActive
                })
                .ToListAsync();

            return users;
        }
    }
}