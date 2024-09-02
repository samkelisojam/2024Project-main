using _2024FinalYearProject.Models.ViewModels;
using _2024FinalYearProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using _2024FinalYearProject.Data.Interfaces;

namespace _2024FinalYearProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepositoryWrapper wrapper;
        private readonly UserManager<AppUser> userManager;

        public AdminController(IRepositoryWrapper repositoryWrapper, UserManager<AppUser> userManager)
        {
            wrapper = repositoryWrapper;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [TempData]
        public string Message { get; set; }

       
       


        public async Task<IActionResult> ViewAllLogins(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var allLogins = await wrapper.Logins.GetAllAsync();
                var userBankAccount = (await wrapper.BankAccount.GetAllAsync()).FirstOrDefault(bc => bc.AccountNumber == user.AccountNumber);
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
            var user = await userManager.FindByEmailAsync(email);

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
                var user = await userManager.FindByEmailAsync(model.UserEmail);
                if (user != null)
                {
                    var AllBankAcc = await wrapper.BankAccount.GetAllAsync();
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
                        await wrapper.BankAccount.UpdateAsync(userBankAcc);
                        var transaction = new Transaction
                        {
                            Amount = model.Amount,
                            UserEmail = model.UserEmail,
                            Reference = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(action)} cash in account",
                            BankAccountIdReceiver = int.Parse(user.AccountNumber),
                            BankAccountIdSender = 0,
                            TransactionDate = DateTime.Now
                        };
                        await wrapper.Transaction.AddAsync(transaction);
                        wrapper.SaveChanges();
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
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var results = await userManager.DeleteAsync(user);
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
            var user = await userManager.FindByEmailAsync(email);
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

        [HttpPost]
        public async Task<IActionResult> ConsultantUpdateUser(ConsultantUpdateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.LastName = model.Lastname;
                    user.DateOfBirth = model.DateOfBirth;
                    var result = await userManager.UpdateAsync(user);
                    Message = "Updated User Details\n";
                    if (result.Succeeded)
                    {
                        if (model.Password != null && model.ConfirmPassword != null && model.Password == model.ConfirmPassword)
                        {
                            var passResults = await userManager.RemovePasswordAsync(user);
                            if (passResults.Succeeded)
                            {
                                if ((await userManager.AddPasswordAsync(user, model.Password)).Succeeded)
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
