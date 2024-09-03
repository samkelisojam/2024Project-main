using _2024FinalYearProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _2024FinalYearProject.Data.SeedData
{
    public static class SeedData
    {
        private static readonly string password = "@Banker123";
        private static readonly AppUser consultantUser = new AppUser
        {
            UserName = "def_consultant",
            FirstName = "Nick",
            LastName = "Cage",
            Email = "nicky@ufs.ac.za",
            DateOfBirth = DateTime.Now,
            IDnumber = "9876543210123",
            StudentStaffNumber = "0123456789",
            AccountNumber = "0000000001",
            UserRole = "Consultant"
        };
        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            AppDbContext context = app.ApplicationServices.CreateScope()
               .ServiceProvider.GetRequiredService<AppDbContext>();

            UserManager<AppUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();

            if (await userManager.FindByNameAsync(consultantUser.UserName) == null)
            {

                if (await roleManager.FindByNameAsync(consultantUser.UserRole) == null)
                    await roleManager.CreateAsync(new(consultantUser.UserRole));

                AppUser user = new()
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
                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "Consultant");
            }
        }
    }
}