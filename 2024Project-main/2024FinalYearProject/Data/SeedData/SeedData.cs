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
        private static readonly AppUser adminUser = new AppUser
        {
            UserName = "def_admin",
            FirstName = "Chuck",
            LastName = "Norris",
            Email = "norris@ufs.ac.za",
            DateOfBirth = DateTime.Now,
            IDnumber = "8876543210123",
            StudentStaffNumber = "9876543210",
            AccountNumber = "0000000002",
            UserRole = "Admin"
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

                IdentityResult result = await CreatePreAppUser(consultantUser, userManager);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(consultantUser, "Consultant");
                result = await CreatePreAppUser(adminUser, userManager);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        public static async Task<IdentityResult> CreatePreAppUser(AppUser user, UserManager<AppUser> userManager)
        {
            AppUser _user = new()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                StudentStaffNumber = user.StudentStaffNumber,
                AccountNumber = user.AccountNumber,
                DateOfBirth = user.DateOfBirth,
                IDnumber = user.IDnumber,
                UserRole = user.UserRole,
            };
            return await userManager.CreateAsync(user, password);
        }
    }
}