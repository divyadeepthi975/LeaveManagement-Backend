using LeaveManagement.Data;
using LeaveManagement.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(
            AppDbContext db)
        {
            var adminExists = await db.Loginuser
                .AnyAsync(x =>
                    x.role == "Admin");

            if (adminExists)
            {
                return;
            }

            using var transaction =
                await db.Database.BeginTransactionAsync();

            try
            {
                var loginUser = new Loginusers
                {
                    username = "admin@techwave.com",
                    role = "Admin"
                };

                var passwordHasher =
                    new PasswordHasher<Loginusers>();

                loginUser.passwordhash =
                    passwordHasher.HashPassword(
                        loginUser,
                        "Admin@123");

                db.Loginuser.Add(loginUser);

                await db.SaveChangesAsync();


                var emp = new Employee
                {
                    EmployeeId = loginUser.employeeid,

                    EmployeeCode = "Emp001",
                    Name = "System Administrator",
                    Email = "admin@techwave.com",
                    Department = "Administration",
                    JoiningDate = DateTime.UtcNow,
                    IsActive = true,
                    loginuser = loginUser
                };

                db.Employees.Add(emp);

                await db.SaveChangesAsync();
            
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}