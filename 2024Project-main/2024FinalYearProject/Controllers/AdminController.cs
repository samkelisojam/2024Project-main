using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;
using _2024FinalYearProject.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _2024FinalYearProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepositoryWrapper _wrapper;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(IRepositoryWrapper wrapper , UserManager<AppUser> userManager )
        {
            _wrapper = wrapper;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var transactions = await _wrapper.Transaction.GetAllAsync();
            var consultants =  (await _userManager.GetUsersInRoleAsync("Consultant")).ToList();
            var users =  (await _userManager.GetUsersInRoleAsync("User")).ToList();

            var indexPageViewModel = new IndexPageViewModel()
            {
                Transactions = transactions ,
                Consultants = consultants,
                Users = users

            };

            return View(indexPageViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users =  await _wrapper.AppUser.GetAllUsersAndBankAccount("Consultant");
            var userPageViewModel = new UserPageViewModel()
            {
                AppUsers = users
            };
            
            return View(userPageViewModel);
        }

        //delete transaction
        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            await _wrapper.Transaction.RemoveAsync(id);
            return RedirectToAction("Index");
        }
    }
}
