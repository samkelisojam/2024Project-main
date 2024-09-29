using _2024FinalYearProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _2024FinalYearProject.Data.SeedData
{
    public static class SeedData
    {
        private static readonly string consultantPassword = "@Banker123";
        private static readonly string adminPassword = "@Admin123";
        private static readonly string finPassword = "@FinAdvisor123"; // Password for the financial advisor

        // Consultant user
        private static readonly AppUser consultantUser = new AppUser
        {
            UserName = "NOLWAZI",
            FirstName = "Nick",
            LastName = "Sonke",
            Email = "ni@ufs.ac.za",
            DateOfBirth = DateTime.Now,
            IDnumber = "9876543210123",
            StudentStaffNumber = "0123456789",
            AccountNumber = "0000000001",
            UserRole = "Consultant"
        };

        // Admin user
        private static readonly AppUser adminUser = new AppUser
        {
            UserName = "admin_user",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@gmail.com",
            DateOfBirth = DateTime.Now,
            IDnumber = "1234567890123",
            StudentStaffNumber = "9876543210",
            AccountNumber = "0000000002",
            UserRole = "Admin"
        };

        // Financial Advisor user
        private static readonly AppUser financialAdvisorUser = new AppUser
        {
            UserName = "finance_advisor",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@finance.com",
            DateOfBirth = DateTime.Now,
            IDnumber = "6543219870123",
            StudentStaffNumber = "4567890123",
            AccountNumber = "0000000003",
            UserRole = "FinAdvisor"
        };

        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            AppDbContext context = app.ApplicationServices.CreateScope()
               .ServiceProvider.GetRequiredService<AppDbContext>();

            UserManager<AppUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();

            // Seed Consultant User
            if (await userManager.FindByNameAsync(consultantUser.UserName) == null)
            {
                if (await roleManager.FindByNameAsync(consultantUser.UserRole) == null)
                    await roleManager.CreateAsync(new IdentityRole(consultantUser.UserRole));

                AppUser user = new AppUser
                {
                    UserName = consultantUser.UserName,
                    FirstName = consultantUser.FirstName,
                    LastName = consultantUser.LastName,
                    Email = consultantUser.Email,
                    StudentStaffNumber = consultantUser.StudentStaffNumber,
                    AccountNumber = consultantUser.AccountNumber,
                    DateOfBirth = consultantUser.DateOfBirth,
                    IDnumber = consultantUser.IDnumber,
                    UserRole = consultantUser.UserRole,
                };
                IdentityResult result = await userManager.CreateAsync(user, consultantPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Consultant");
            }

            // Seed Admin User
            if (await userManager.FindByNameAsync(adminUser.UserName) == null)
            {
                if (await roleManager.FindByNameAsync(adminUser.UserRole) == null)
                    await roleManager.CreateAsync(new IdentityRole(adminUser.UserRole));

                AppUser user = new AppUser
                {
                    UserName = adminUser.UserName,
                    FirstName = adminUser.FirstName,
                    LastName = adminUser.LastName,
                    Email = adminUser.Email,
                    StudentStaffNumber = adminUser.StudentStaffNumber,
                    AccountNumber = adminUser.AccountNumber,
                    DateOfBirth = adminUser.DateOfBirth,
                    IDnumber = adminUser.IDnumber,
                    UserRole = adminUser.UserRole,
                };
                IdentityResult result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Admin");
            }

            // Seed Financial Advisor User
            if (await userManager.FindByNameAsync(financialAdvisorUser.UserName) == null)
            {
                if (await roleManager.FindByNameAsync(financialAdvisorUser.UserRole) == null)
                    await roleManager.CreateAsync(new IdentityRole(financialAdvisorUser.UserRole));

                AppUser user = new AppUser
                {
                    UserName = financialAdvisorUser.UserName,
                    FirstName = financialAdvisorUser.FirstName,
                    LastName = financialAdvisorUser.LastName,
                    Email = financialAdvisorUser.Email,
                    StudentStaffNumber = financialAdvisorUser.StudentStaffNumber,
                    AccountNumber = financialAdvisorUser.AccountNumber,
                    DateOfBirth = financialAdvisorUser.DateOfBirth,
                    IDnumber = financialAdvisorUser.IDnumber,
                    UserRole = financialAdvisorUser.UserRole,
                };
                IdentityResult result = await userManager.CreateAsync(user, finPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "FinAdvisor");
            }
        }
    }
}