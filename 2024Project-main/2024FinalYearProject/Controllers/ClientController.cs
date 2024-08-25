
using _2024FinalYearProject.Data;
using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;

using _2024FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {

                return NotFound();
            }
            var allNotifications = await _repo.Notification.GetAllAsync();

            var userNotifications = allNotifications.Where(n => n.AppUserId == user.Id).ToList();

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

            return RedirectToAction("CashSentSuccess", new { cashDigit, pinNumber, transactionDate = transaction.TransactionDate });
        }

     
        public IActionResult CashSentSuccess(string cashDigit, string pinNumber, DateTime transactionDate)
        {
            ViewBag.CashDigit = cashDigit;
            ViewBag.PinNumber = pinNumber;
            ViewBag.TransactionDate = transactionDate;
            return View();
        }

      
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
        public IActionResult AddRating()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRatingAsync(FeedBack feedback)
        {
            if (ModelState.IsValid)
            {
                await _repo.Review.AddAsync(feedback);
                return RedirectToAction("Index", "Client");
            }
            return View(feedback);
        }


        public async Task<IActionResult> ViewBankBalance()
        {
            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);
            var allBankAccount = await _repo.BankAccount.GetAllAsync();
            var bankAccount = allBankAccount.FirstOrDefault(b => b.AppUserId == user.Id && b.AccountOrder == 1);
            var viewModel = new BankAccountViewModela
            {
                AccountNumber = bankAccount.AccountNumber,
                Balance = bankAccount.Balance,

            };

            return View(viewModel);
        }



        public async Task<bool> TransferMoney(int senderBankAccountId, int receiverBankAccountId, decimal amount)
        {

            var senderBankAccount = await _repo.BankAccount.GetByIdAsync(senderBankAccountId);
            var receiverBankAccount = await _repo.BankAccount.GetByIdAsync(receiverBankAccountId);

            if (senderBankAccount.Balance < amount)
            {
                return false;
            }

            senderBankAccount.Balance -= amount;
            await _repo.BankAccount.UpdateAsync(senderBankAccount);

            receiverBankAccount.Balance += amount;
            await _repo.BankAccount.UpdateAsync(receiverBankAccount);


            var senderTransaction = new Transaction
            {
                BankAccountIdSender = senderBankAccountId,
                BankAccountIdReceiver = receiverBankAccountId,
                Amount = -amount,
                TransactionDate = DateTime.UtcNow,
                AppUserId = senderBankAccount.AppUserId
            };

            //  new transaction record for the receiver
            var receiverTransaction = new Transaction
            {
                BankAccountIdSender = senderBankAccountId,
                BankAccountIdReceiver = receiverBankAccountId,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                AppUserId = receiverBankAccount.AppUserId 
            };


            await _repo.Transaction.AddAsync(senderTransaction);
            await _repo.Transaction.AddAsync(receiverTransaction);
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> TransferMoneyview()
        {
           

            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            var allBankAccount = await _repo.BankAccount.GetAllAsync();
            var mainBankAccount = allBankAccount.FirstOrDefault(b => b.AppUserId == user.Id && b.AccountOrder == 1);
            //
        
            

            var viewModel = new MoneyTransferViewModel
            {
                SenderBankAccountId = mainBankAccount.Id,
             
           AvailableBalance = mainBankAccount.Balance,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TransferMoneyview(MoneyTransferViewModel model)
        {
            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            var allBankAccount = await _repo.BankAccount.GetAllAsync();
            var mainBankAccount = allBankAccount.FirstOrDefault(b => b.AppUserId == user.Id && b.AccountOrder == 1);


            int senderBankAccountId = mainBankAccount.Id; 
            int receiverBankAccountId = model.ReceiverBankAccountId;
            decimal amount = model.Amount;



         

      
            var currentUserId = user.Id;

            // Check if the sender's bank account belongs to the current user
            var senderBankAccount = await _repo.BankAccount.GetByIdAsync(senderBankAccountId);
            if (senderBankAccount == null || senderBankAccount.AppUserId != currentUserId)
            {
                return BadRequest("Invalid sender bank account.");
            }

      
            var receiverBankAccount = await _repo.BankAccount.GetByIdAsync(receiverBankAccountId);
            if (receiverBankAccount == null)
            {
                return BadRequest("Invalid receiver bank account.");
            }

           
            bool done = await TransferMoney(senderBankAccountId, receiverBankAccountId, amount);
            if (done)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

    }

}
