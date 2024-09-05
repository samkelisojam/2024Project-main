using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;
using _2024FinalYearProject.Models.ViewModels;
using _2024FinalYearProject.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace _2024FinalYearProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRepositoryWrapper _wrapper;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(IRepositoryWrapper wrapper, UserManager<AppUser> userManager)
        {
            _wrapper = wrapper;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string currentPage = "index")
        {
            var transactions = await _wrapper.Transaction.GetAllAsync();
            var consultants = (await _userManager.GetUsersInRoleAsync("Consultant")).ToList();
            var users = (await _userManager.GetUsersInRoleAsync("User")).ToList();

            var indexPageViewModel = new IndexPageViewModel()
            {
                CurrentPage = currentPage,
                Transactions = transactions,
                Consultants = consultants,
                Users = users
            };

            return View(indexPageViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Users()
        {


            var users = await _wrapper.AppUser.GetAllUsersAndBankAccount();
            var userPageViewModel = new UserPageViewModel()
            {
                AppUsers = users
            };

            return View(userPageViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Consultants()
        {


            var users = (await _userManager.GetUsersInRoleAsync("Consultant")).ToList();

            return View(users);
        }

        //delete transaction
        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            await _wrapper.Transaction.RemoveAsync(id);
            return RedirectToAction("Index");
        }

        [TempData]
        public string Message { get; set; }





        public async Task<IActionResult> ViewAllLogins(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var allLogins = await _wrapper.Logins.GetAllAsync();
                var userBankAccount = (await _wrapper.BankAccount.GetAllAsync()).FirstOrDefault(bc => bc.AccountNumber == user.AccountNumber);
                return View(new ConsultantViewModel
                {
                    SelectedUser = user,
                    loginSessions = allLogins.Where(u => u.UserEmail == email).OrderBy(l => l.TimeStamp)
                });
            }
            return View("Index");
        }
        public async Task<IActionResult> DepositWithdraw(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return View(new ConsultantDepositModel
                {
                    AccountNumber = user.AccountNumber,
                    UserEmail = user.Email,
                });
            }
            return RedirectToAction("Index", "Consultant");
        }

        [HttpPost]
        public async Task<IActionResult> DepositWithdraw(ConsultantDepositModel model, string action)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.UserEmail);
                if (user != null)
                {
                    var AllBankAcc = await _wrapper.BankAccount.GetAllAsync();
                    var userBankAcc = AllBankAcc.FirstOrDefault(bc => bc.UserEmail == user.Email);
                    if (userBankAcc != null)
                    {
                        if (action.ToLower() == "deposit")
                        {
                            userBankAcc.Balance += model.Amount;
                        }
                        else
                        {
                            if (userBankAcc.Balance - model.Amount < -50)
                            {
                                ModelState.AddModelError("", "User has insuffecient balance in their account");
                                return View(model);
                            }
                            userBankAcc.Balance -= model.Amount;
                        }
                        await _wrapper.BankAccount.UpdateAsync(userBankAcc);
                        var transaction = new Transaction
                        {
                            Amount = model.Amount,
                            UserEmail = model.UserEmail,
                            Reference = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(action)} cash in account",
                            BankAccountIdReceiver = int.Parse(user.AccountNumber),
                            BankAccountIdSender = 0,
                            TransactionDate = DateTime.Now
                        };
                        await _wrapper.Transaction.AddAsync(transaction);
                        _wrapper.SaveChanges();
                        Message = $"Money Successfully {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(action)} to account";
                        return RedirectToAction("Index", "Consultant");
                    }
                    else
                    {
                        Message = "Couldn't find bank account, please contact system administrator";
                        ModelState.AddModelError("", "Couldn't find bank account");
                    }
                }
                else
                {
                    Message = "Couldn't find user, please contact system administrator";
                    ModelState.AddModelError("", "Couldn't find user");
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangePassword()
        {
            return View();
        }

        public async Task<IActionResult> ConsultantDeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var results = await _userManager.DeleteAsync(user);
                if (results.Succeeded)
                {
                    return RedirectToAction("Index", "Consultant");
                }
                return View();
            }
            return View();
        }
        public async Task<IActionResult> ConsultantUpdateUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return View(new ConsultantUpdateUserModel
                {
                    AccountNumber = user.AccountNumber,
                    DateOfBirth = user.DateOfBirth,
                    Email = user.Email,
                    IDNumber = user.IDnumber,
                    Lastname = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GenerateReport()
        {
            try
            {
                List<string> data = new List<string>();
                var reportContent = $"Banking Application\n{DateTime.Now:yyyyMMddHHmmss}\n\n" +
                    $"***Users\\Clients***\n" +
                    $"=====================\n" +
                    $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
                var report = _userManager.Users;
                foreach (var u in report)
                {
                    if (await _userManager.IsInRoleAsync(u, "User"))
                    {
                        data.Add($"{u.AccountNumber}\t{u.FirstName}\t{u.LastName}\t{u.Email}\t{u.StudentStaffNumber}\n");
                    }
                }
                reportContent += string.Join('\n', data.ToArray());


                reportContent += $"\n***All Transactions***\n" +
                                 $"==========================\n" +
                    $"Account No\tFirst Name\tLast Name\tEmail Address\tStudent Number\n\n";
                var transactions = await _wrapper.Transaction.GetAllAsync();
                reportContent += string.Join('\n', transactions.Select(u => $"{u.UserEmail}\t{u.Amount}\t{u.BankAccountIdReceiver}\t{u.BankAccountIdSender}\n").ToArray());

                var contentBytes = Encoding.UTF8.GetBytes(reportContent);
                var fileName = $"Report_{DateTime.Now:yyyyMMddHHmmss}.txt";

                return File(contentBytes, "text/plain", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error generating report: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConsultantUpdateUser(ConsultantUpdateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.LastName = model.Lastname;
                    user.DateOfBirth = model.DateOfBirth;
                    var result = await _userManager.UpdateAsync(user);
                    Message = "Updated User Details\n";
                    if (result.Succeeded)
                    {
                        if (model.Password != null && model.ConfirmPassword != null && model.Password == model.ConfirmPassword)
                        {
                            var passResults = await _userManager.RemovePasswordAsync(user);
                            if (passResults.Succeeded)
                            {
                                if ((await _userManager.AddPasswordAsync(user, model.Password)).Succeeded)
                                {
                                    Message += "Successfully updated password";
                                }
                                else
                                {
                                    Message += "Error updating password...Skipping process";
                                }
                            }
                        }
                        return RedirectToAction("Index", "Consultant");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Could not find user, please contact system admin");
                    Message = "Could not find user, please contact system admin";
                    return View(model);
                }
            }
            return View(model);
        }
    }
}
