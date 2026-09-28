using LeaveManagement.Data;
using LeaveManagement.DTO;
using LeaveManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        

        public async Task<LoginResponseDTO> LoginAsync(
            LoginDTO loginDTO)
        {
            var loginUser = await _db.Loginuser
                .Include(u => u.employee)
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
            if (!loginUser.employee!.IsActive)
            {
                throw new UnauthorizedAccessException(
                    "User is not active");
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
    }
}