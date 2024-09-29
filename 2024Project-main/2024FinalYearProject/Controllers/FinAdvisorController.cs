using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;
using _2024FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _2024FinalYearProject.Controllers
{
   
    public class FinAdvisorController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IRepositoryWrapper wrapper;

        public FinAdvisorController(UserManager<AppUser> _userManager, IRepositoryWrapper wrapper)
        {
            userManager = _userManager;
            this.wrapper = wrapper;
        }


        [TempData]
        public string Message { get; set; }

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

        [HttpGet]
        public async Task<IActionResult> Advice(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                Message = "Could not Find User, Please Try Again";
                return RedirectToAction("Index", "FinAdvisor");
            }

            var allTransactions = (await wrapper.Transaction.GetAllAsync()).Where(t => t.UserEmail == email).ToList();
            var currentUserBankAccount = (await wrapper.BankAccount.GetAllAsync()).Where(ba => ba.UserEmail == email).FirstOrDefault();

            return View(new AdvisorViewModel
            {
                UserEmail = user.Email,
                CurrentUser = user,
                Transactions = allTransactions,
                CurrentUserBankAccount = currentUserBankAccount,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Advice(AdvisorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var notify = new Notification
                {
                    Message = "[ADVICE]" + " " + model.Advise,
                    UserEmail = model.UserEmail,
                    NotificationDate = DateTime.Now,
                    IsRead = false
                };
                await wrapper.Notification.AddAsync(notify);
                wrapper.SaveChanges();
            }
            Message = "Successfully send advice to user";
            return RedirectToAction("Index", "FinAdvisor");
        }

    }
}