﻿using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;

using _2024FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        [TempData]
        public string Message { get; set; }
        public async Task<IActionResult> IndexAsync()
        {
            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);
            var model = new UpdateProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccountNumber = user.AccountNumber,
                IDNumber = user.IDnumber,
                Userrole = user.UserRole,
                Lastname = user.LastName + " " + user.FirstName,
            };
            return View(model);

        }

        public async Task<IActionResult> NotificationMessage()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var allNotifications = await _repo.Notification.GetAllAsync();
            var userNotifications = allNotifications.Where(n => n.UserEmail == user.Email).ToList();

            foreach (var notification in userNotifications)
            {
                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    await _repo.Notification.UpdateAsync(notification);
                }
            }

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
            var allTransaction = await _repo.Transaction.GetAllAsync();

            var userTransaction = allTransaction.Where(n => n.UserEmail == user.Email).ToList();

            return View(userTransaction);
        }

        [HttpGet]
        public async Task<IActionResult> CashSent()
        {
            var username = User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            var allBankAccount = await _repo.BankAccount.GetAllAsync();
            var bankAccount = allBankAccount.FirstOrDefault(b => b.UserEmail == user.Email && b.AccountOrder == 1);

            if (bankAccount == null)
            {
                return View("NotFound");
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
            var bankAccount = allBankAccount.FirstOrDefault(b => b.UserEmail == user.Email && b.AccountOrder == 1);

            if (bankAccount == null || bankAccount.Balance < model.Amount)
            {
                ModelState.AddModelError("", "Insufficient balance or account not found.");
                return View(model);
            }

            bankAccount.Balance -= model.Amount;

            var transaction = new Transaction
            {
                BankAccountIdSender = bankAccount.Id,
                BankAccountIdReceiver = 0,
                Amount = model.Amount,
                TransactionDate = DateTime.Now,
                UserEmail = user.Email,
                Reference = "CashSent",
            };

            var cashDigit = GenerateRandomNumber(13);
            var pinNumber = GenerateRandomNumber(4);

            await _repo.Transaction.AddAsync(transaction);
            var notification = new Notification
            {
                Message = $"You have successfully sent cash. Amount: {model.Amount:C}.",
                NotificationDate = DateTime.Now,
                IsRead = false,
                UserEmail = user.Email
            };
            await _repo.Notification.AddAsync(notification);

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
        public async Task<IActionResult> AddRating(FeedBack feedback)
        {
            var currentLoginUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (User != null)
            {
                feedback.UserEmail = currentLoginUser.Email;
                feedback.dateTime = DateTime.Now;
                if (ModelState.IsValid)
                {
                    await _repo.Review.AddAsync(feedback);
                    Message = "Review sent successfully";
                    return RedirectToAction("Index", "Client");
                }
            }
            Message = "There was an error sending the review";
            return View(feedback);
        }

        public async Task<IActionResult> ViewBankBalance()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var allBankAccount = await _repo.BankAccount.GetAllAsync();
            var bankAccount = allBankAccount.FirstOrDefault(b => b.UserEmail == user.Email && b.AccountOrder == 1);
            var viewModel = new BankAccountViewModela
            {
                AccountNumber = bankAccount.AccountNumber,
                Balance = bankAccount.Balance,

            };
            return View(viewModel);
        }

        public async Task<bool> TransferMoney(string senderAccountNumber, string receiverAccountNumber, decimal amount)
        {

            var allBankAccounts = await _repo.BankAccount.GetAllAsync();

            var senderBankAccount = allBankAccounts.FirstOrDefault(b => b.AccountNumber == senderAccountNumber);
            var receiverBankAccount = allBankAccounts.FirstOrDefault(b => b.AccountNumber == receiverAccountNumber);

            if (senderBankAccount == null || receiverBankAccount == null)
            {
                return false;
            }


            if (senderBankAccount.Balance < amount)
            {
                return false;
            }


            senderBankAccount.Balance -= amount;
            receiverBankAccount.Balance += amount;


            await _repo.BankAccount.UpdateAsync(senderBankAccount);
            await _repo.BankAccount.UpdateAsync(receiverBankAccount);


            var senderNotification = new Notification
            {
                Message = $"You have sent {amount:C} to account {receiverBankAccount.AccountNumber}.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = senderBankAccount.UserEmail
            };
            var receiverNotification = new Notification
            {
                Message = $"You have received {amount:C} from account {senderBankAccount.AccountNumber}.",
                NotificationDate = DateTime.UtcNow,
                IsRead = false,
                UserEmail = receiverBankAccount.UserEmail
            };

            await _repo.Notification.AddAsync(senderNotification);
            await _repo.Notification.AddAsync(receiverNotification);
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> TransferMoneyView()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            var allBankAccounts = await _repo.BankAccount.GetAllAsync();
            var mainBankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == user.Email && b.AccountOrder == 1);

            var viewModel = new MoneyTransferViewModel
            {
                SenderBankAccountNumber = mainBankAccount.AccountNumber,
                AvailableBalance = mainBankAccount.Balance,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TransferMoneyView(MoneyTransferViewModel model)
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();
            var mainBankAccount = allBankAccounts.FirstOrDefault(b => b.UserEmail == user.Email && b.AccountOrder == 1);

            string senderAccountNumber = mainBankAccount.AccountNumber;
            string receiverAccountNumber = model.ReceiverBankAccountNumber;
            decimal amount = model.Amount;


            bool transferSuccess = await TransferMoney(senderAccountNumber, receiverAccountNumber, amount);

            if (transferSuccess)
            {
                return RedirectToAction("TransferSuccess", new { amount = amount, receiverAccount = receiverAccountNumber });
            }
            else
            {
                return View("NotFound");

            }
        }
        public IActionResult TransferSuccess(decimal amount, string receiverAccount)
        {
            var model = new TransferSuccessViewModel
            {
                Amount = amount,
                ReceiverAccount = receiverAccount
            };
            return View(model);
        }
    }
}
