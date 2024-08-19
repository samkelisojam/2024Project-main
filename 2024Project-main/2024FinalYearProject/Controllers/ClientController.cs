



using _2024FinalYearProject.Data;
using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2024FinalYearProject.Controllers
{
    public class ClientController : Controller
    {
        private readonly IRepositoryWrapper _repo;
        private readonly UserManager<AppUser> _userManager;

        public ClientController(IRepositoryWrapper repo, UserManager<AppUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Get Notification
        public async Task<IActionResult> NotificationMessage()
        {
            // Get the username of the currently logged-in user
            var username = User.Identity.Name;

            // Find the user in the database using the username (await the async call)
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                // Handle the case where the user is not found, perhaps return an error or redirect
                return NotFound();
            }

            // Get all notifications (assumes _repo.Notification.GetAllAsync() is an async method)
            var allNotifications = await _repo.Notification.GetAllAsync();

            // Filter notifications by the user's ID
            var userNotifications = allNotifications.Where(n => n.AppUserId == user.Id).ToList();

            // Return the filtered list to the view
            return View(userNotifications);
        }
        [HttpGet]
        public async Task<IActionResult> Transactions()
        {
         
            var username = User.Identity.Name;

          
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
              
                return NotFound();
            }

          
            var allTransaction = await _repo.Notification.GetAllAsync();

          
            var userTransaction = allTransaction.Where(n => n.AppUserId == user.Id).ToList();

            return View(userTransaction);
        }

        // My Account (Commented Out)
        // public IActionResult MyAccount()
        // {
        //     var transactions = _repo.Transaction.GetAll();
        //     var bankBalance = _repo.BankBalance.GetAll();
        //
        //     var viewModel = new BankAccountViewModel
        //     {
        //         BankAccount = bankBalance,
        //         Transactions = transactions
        //     };
        //     return View(viewModel);
        // }

        [HttpGet]
        public async Task<IActionResult> CashSent()
        {
            var username = User.Identity.Name;


            var user = await _userManager.FindByNameAsync(username);


            var allBankAccount = await _repo.BankAccount.GetAllAsync();
            var bankAccount = allBankAccount.FirstOrDefault(b => b.AppUserId == user.Id && b.AccountOrder == 1);

            if (bankAccount == null)
            {
                return NotFound("Main bank account not found.");
            }

            var model = new CashSentViewModel
            {
                BankAccountId = bankAccount.Id,
                AvailableBalance = bankAccount.Balance
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CashSent(CashSentViewModel model)
        {
            var username = User.Identity.Name;


            var user = await _userManager.FindByNameAsync(username);


            var allBankAccount = await _repo.BankAccount.GetAllAsync();
            var bankAccount = allBankAccount.FirstOrDefault(b => b.AppUserId == user.Id && b.AccountOrder == 1);

            if (bankAccount == null || bankAccount.Balance < model.Amount)
            {
                ModelState.AddModelError("", "Insufficient balance or account not found.");
                return View(model);
            }

            // Deduct the amount from the sender's account
            bankAccount.Balance -= model.Amount;

            // Generate Cash Sent details
            var transaction = new Transaction
            {
                BankAccountIdSender = bankAccount.Id,
                BankAccountIdReceiver = 0, // Assuming 0 for cashless transactions
                Amount = model.Amount,
                TransactionDate = DateTime.Now,
                Reference = "CashSent",
            };

            // Generate random Cash Digit and PIN
            var cashDigit = GenerateRandomNumber(13);
            var pinNumber = GenerateRandomNumber(4);

            // Save the transaction
            await _repo.Transaction.AddAsync(transaction);
           await _repo.Transaction.SaveAsync();

            // Prepare and return the success view
            return RedirectToAction("CashSentSuccess", new { cashDigit, pinNumber, transactionDate = transaction.TransactionDate });
        }

        // Display Success View
        public IActionResult CashSentSuccess(string cashDigit, string pinNumber, DateTime transactionDate)
        {
            ViewBag.CashDigit = cashDigit;
            ViewBag.PinNumber = pinNumber;
            ViewBag.TransactionDate = transactionDate;
            return View();
        }

        // Utility method to generate random numbers as strings
        private string GenerateRandomNumber(int length)
        {
            var random = new Random();
            string result = string.Empty;
            for (int i = 0; i < length; i++)
            {
                result += random.Next(0, 10).ToString();
            }
            return result;
        }

        [HttpGet]
        public  IActionResult AddRating()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRatingAsync(FeedBack feedback)
        {
            if (ModelState.IsValid)
            {
               await  _repo.Review.AddAsync(feedback);
                await _repo.Review.SaveAsync();
                return  RedirectToAction("Index", "Client");
            }

            return View(feedback);
        }
    }

}
