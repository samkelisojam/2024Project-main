using _2024FinalYearProject.Models;
using _2024FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _2024FinalYearProject.Controllers
{
    [Authorize(Roles = "Consultant")]
    public class ConsultantController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public ConsultantController(UserManager<AppUser> _userManager)
        {
            userManager = _userManager;
        }


        public async Task<IActionResult> Index()
        {
            List<AppUser> lstUsers = new List<AppUser>();
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "User"))
                    lstUsers.Add(user);
            }
            return View(new ConsultantViewModel
            {
                appUsers = lstUsers.AsQueryable()
            });
        }

        public IActionResult ChangePassword(string userId)
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateReport()
        {
            //Please add code here
            return View();
        }
    }
}