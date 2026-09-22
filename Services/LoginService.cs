using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LeaveManagement.Services
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IEmployeeService _employeeService;

        private readonly PasswordHasher<Loginusers>
            _passwordHasher;

        public LoginService(
            AppDbContext db,
            IConfiguration configuration,
            IEmployeeService employeeService)
        {
            _db = db;
            _configuration = configuration;
            _employeeService = employeeService;

            _passwordHasher =
                new PasswordHasher<Loginusers>();
        }

        public async Task<LoginResponseDTO> RegisterEmployeeAsync(
            RegisterEmployeeDTO registerDTO)
        {
            string role = registerDTO.role.Trim().ToLower();

            if (role != "manager" && role != "employee")
            {
                throw new Exception(
                    "Role must be Manager or Employee.");
            }

            bool usernameExists = await _db.Loginuser
                .AnyAsync(l =>
                    l.username == registerDTO.username);

            if (usernameExists)
            {
                throw new Exception(
                    "Username already exists.");
            }

            int employeeId = await GetNextEmployeeIdAsync();

            await using var transaction =
                await _db.Database.BeginTransactionAsync();

            try
            {
                // 1. Create Loginusers record first
                var loginUser = new Loginusers
                {
                    employeeid = employeeId,
                    username = registerDTO.username,
                    role = role
                };

                loginUser.passwordhash =
                    _passwordHasher.HashPassword(
                        loginUser,
                        registerDTO.password);

                _db.Loginuser.Add(loginUser);

                await _db.SaveChangesAsync();

                // 2. Prepare employee DTO
                var employeeDTO = new EmployeeDTO
                {
                    EmployeeCode = registerDTO.EmployeeCode,
                    Name = registerDTO.Name,
                    Email = registerDTO.Email,
                    Department = registerDTO.Department,
                    JoiningDate = registerDTO.JoiningDate
                };

                // 3. Create Employee using EmployeeService
                await _employeeService.AddWithIdAsync(
                    employeeDTO,
                    employeeId);

                // 4. Commit both records
                await transaction.CommitAsync();

                // 5. Generate JWT token
                string token = GenerateToken(loginUser);

                return new LoginResponseDTO
                {
                    token = token,
                    employeeid = employeeId,
                    username = loginUser.username,
                    role = loginUser.role
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<LoginResponseDTO> LoginAsync(
            LoginDTO loginDTO)
        {
            var loginUser = await _db.Loginuser
                .FirstOrDefaultAsync(l =>
                    l.username == loginDTO.username);

            if (loginUser == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    loginUser,
                    loginUser.passwordhash,
                    loginDTO.password);

            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Invalid username or password.");
            }

            string token = GenerateToken(loginUser);

            return new LoginResponseDTO
            {
                token = token,
                employeeid = loginUser.employeeid,
                username = loginUser.username,
                role = loginUser.role
            };
        }

        private string GenerateToken(Loginusers loginUser)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    loginUser.employeeid.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    loginUser.username),

                new Claim(
                    ClaimTypes.Role,
                    loginUser.role)
            };

            string? keyValue =
                _configuration["Jwt:Key"];

            string? issuer =
                _configuration["Jwt:Issuer"];

            string? audience =
                _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(keyValue) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new Exception(
                    "JWT configuration is missing.");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyValue));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
        private async Task<int> GetNextEmployeeIdAsync()
        {
            await using var connection =
                _db.Database.GetDbConnection();

            if (connection.State !=
                System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var command =
                connection.CreateCommand();

            command.CommandText =
                "SELECT NEXT VALUE FOR EmployeeIdSequence";

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }
    }
}