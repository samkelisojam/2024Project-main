using _2024FinalYearProject.Data.Interfaces;
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
            // Get the logged-in user's username
            var username = User.Identity.Name;


            var user = await _userManager.FindByNameAsync(username);

            // Get all bank accounts for the user
            var allBankAccounts = await _repo.BankAccount.GetAllAsync();


            var userBankAccounts = allBankAccounts.Where(b => b.UserEmail == user.Email).ToList();


            var transactions = await _repo.Transaction.GetAllAsync();
            var userTransactions = transactions.Where(t => userBankAccounts.Any(b => b.UserEmail == user.Email)).ToList();


            var viewModel = new BankAccountViewModel
            {
                BankAccount = userBankAccounts,
                Transactions = userTransactions
            };

            // Return the view with the viewModel
            return View(viewModel);
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
