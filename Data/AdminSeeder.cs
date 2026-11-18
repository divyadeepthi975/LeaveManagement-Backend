using System.ComponentModel.DataAnnotations;
using System.Data;
using LeaveManagement.Data;
using LeaveManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeaveManagement
{
    public static class AdminSeeder
    {
        public static async Task BootstrapInitialAdminAsync(
            AppDbContext db,
            IConfiguration configuration,
            ILogger logger)
        {
            var enabledValue = configuration["InitialAdmin:Enabled"];
            if (!string.IsNullOrWhiteSpace(enabledValue) &&
                !bool.TryParse(enabledValue, out _))
            {
                throw new InvalidOperationException(
                    "InitialAdmin__Enabled must be true or false.");
            }

            var bootstrapEnabled = bool.TryParse(enabledValue, out var enabled) && enabled;

            await using var transaction =
                await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            if (await db.Loginuser.AnyAsync(user => user.role == "Admin"))
            {
                logger.LogInformation(
                    "Initial admin bootstrap skipped because an admin account already exists.");
                await transaction.CommitAsync();
                return;
            }

            if (!bootstrapEnabled)
            {
                logger.LogWarning(
                    "No admin account exists. Set InitialAdmin__Enabled=true and provide InitialAdmin__Email, InitialAdmin__Password, InitialAdmin__EmployeeCode, InitialAdmin__Name, and InitialAdmin__Department to bootstrap one.");
                await transaction.CommitAsync();
                return;
            }

            var email = configuration["InitialAdmin:Email"]?.Trim() ?? string.Empty;
            var password = configuration["InitialAdmin:Password"] ?? string.Empty;
            var employeeCode = configuration["InitialAdmin:EmployeeCode"]?.Trim() ?? string.Empty;
            var name = configuration["InitialAdmin:Name"]?.Trim() ?? string.Empty;
            var department = configuration["InitialAdmin:Department"]?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(employeeCode) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(department))
            {
                throw new InvalidOperationException(
                    "Initial admin bootstrap is enabled, but one or more required InitialAdmin settings are missing.");
            }

            if (password.Length < 12)
            {
                throw new InvalidOperationException(
                    "InitialAdmin__Password must be at least 12 characters.");
            }

            if (await db.Loginuser.AnyAsync(user => user.username == email) ||
                await db.Employees.AnyAsync(employee =>
                    employee.EmployeeCode == employeeCode ||
                    employee.Email == email))
            {
                throw new InvalidOperationException(
                    "The initial admin email or employee code is already in use.");
            }

            var loginUser = new Loginusers
            {
                username = email,
                role = "Admin"
            };

            loginUser.passwordhash = new PasswordHasher<Loginusers>()
                .HashPassword(loginUser, password);

            var employee = new Employee
            {
                EmployeeCode = employeeCode,
                Name = name,
                Email = email,
                Department = department,
                JoiningDate = DateTime.UtcNow,
                IsActive = true,
                ManagerId = null,
                loginuser = loginUser
            };

            ValidateEntity(loginUser, "Initial admin login configuration");
            ValidateEntity(employee, "Initial admin profile configuration");

            db.Loginuser.Add(loginUser);
            await db.SaveChangesAsync();

            employee.EmployeeId = loginUser.employeeid;
            db.Employees.Add(employee);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();
            logger.LogInformation("Initial admin account was bootstrapped.");
        }

        private static void ValidateEntity(object entity, string description)
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(
                    entity,
                    new ValidationContext(entity),
                    validationResults,
                    validateAllProperties: true))
            {
                var errors = string.Join(
                    "; ",
                    validationResults.Select(result => result.ErrorMessage));

                throw new InvalidOperationException(
                    $"{description} is invalid: {errors}");
            }
        }
    }
}
