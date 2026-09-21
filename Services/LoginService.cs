using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.DTOs;
using LeaveManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeaveManagement.Services
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Loginusers> _passwordHasher;

        public LoginService(
            AppDbContext db,
            IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<Loginusers>();
        }


        public async Task<LoginResponseDTO> RegisterEmployeeAsync(RegisterEmployeeDTO registerDTO)
        {
            // Validate role
            string role = registerDTO.role.Trim();

            if (role != "Manager" && role != "Employee")
            {
                throw new ArgumentException(
                    "Role must be Manager or Employee.");
            }

            // Validate username uniqueness
            bool usernameExists = await _db.Loginuser
                .AnyAsync(l => l.username == registerDTO.username);

            if (usernameExists)
            {
                throw new Exception("Username already exists.");
            }

            // Validate employee code uniqueness
            bool employeeCodeExists = await _db.Employees
                .AnyAsync(e =>
                    e.EmployeeCode == registerDTO.EmployeeCode);

            if (employeeCodeExists)
            {
                throw new Exception("Employee code already exists.");
            }

            // Validate employee email uniqueness
            bool emailExists = await _db.Employees
                .AnyAsync(e => e.Email == registerDTO.Email);

            if (emailExists)
            {
                throw new Exception("Email already exists.");
            }

            // Generate employee ID using SQL Server sequence
            int employeeId = await _db.Database
                .SqlQuery<int>(
                    $"SELECT NEXT VALUE FOR EmployeeIdSequence AS Value")
                .SingleAsync();

            await using var transaction =
                await _db.Database.BeginTransactionAsync();

            try
            {
                // 1. Create Loginusers first
                var loginUser = new Loginusers
                {
                    employeeid = employeeId,
                    username = registerDTO.username.Trim(),
                    role = role
                };

                loginUser.passwordhash =
                    _passwordHasher.HashPassword(
                        loginUser,
                        registerDTO.password);

                _db.Loginuser.Add(loginUser);

                await _db.SaveChangesAsync();

                // 2. Create Employee using the same ID
                var employee = new Employee
                {
                    EmployeeId = employeeId,
                    EmployeeCode = registerDTO.EmployeeCode,
                    Name = registerDTO.Name,
                    Email = registerDTO.Email,
                    Department = registerDTO.Department,
                    JoiningDate = registerDTO.JoiningDate,
                    IsActive = true
                };

                _db.Employees.Add(employee);

                await _db.SaveChangesAsync();

                // 3. Initialize leave balances
                var leaveTypes = await _db.Leavetypes
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

                // 4. Commit transaction
                await transaction.CommitAsync();

                return new LoginResponseDTO
                {
                    employeeid = employeeId,
                    username = loginUser.username,
                    role = loginUser.role,
                    token = GenerateJwtToken(loginUser)
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}