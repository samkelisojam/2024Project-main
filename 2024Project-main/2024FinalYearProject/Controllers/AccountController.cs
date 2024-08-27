using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;
using _2024FinalYearProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _2024FinalYearProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRepositoryWrapper wrapper;
        private readonly string role = "User";

        public AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager,
            RoleManager<IdentityRole> _roleManager, IRepositoryWrapper _wrapper)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            wrapper = _wrapper;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string registerAs = "student")
        {
            return View(new RegisterViewModel() { RegisterAs = registerAs });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                    await roleManager.CreateAsync(new(role));

                AppUser user = new()
                {
                    UserName = (registerModel.LastName + registerModel.FirstName).Substring(0, 10),
                    IDnumber = registerModel.IdPassportNumber,
                    Email = registerModel.EmailAddress,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    StudentStaffNumber = registerModel.StudentStaffNumber,
                    UserRole = registerModel.RegisterAs
                };

                Random rndAccount = new Random();
                string _randomAccount = string.Empty;
                do
                { _randomAccount = rndAccount.Next(99999999, 999999999).ToString(); }
                while (userManager.Users.Where(u => u.AccountNumber != _randomAccount).FirstOrDefault() == null);
                user.AccountNumber = _randomAccount;

                BankAccount bankAccountMain = new()
                {
                    AccountNumber = _randomAccount,
                    Balance = 600m,
                    BankAccountType = "Savings",
                    AccountOrder = 1,
                    AppUserId = user.Id,

                };
                wrapper.BankAccount.AddAsync(bankAccountMain);
                Transaction transaction = new()
                {
                    BankAccountIdReceiver = int.Parse(_randomAccount),
                    Amount = 600m,
                    Reference = "fee Open new account ",
                    AppUserId = user.Id,

                };
                wrapper.Transaction.AddAsync(transaction);



                IdentityResult result = await userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    var signin_result = await signInManager.PasswordSignInAsync(user, registerModel.Password,
                        isPersistent: false, lockoutOnFailure: false);
                    if (signin_result.Succeeded)
                    {
                        if (await userManager.IsInRoleAsync(user, "Consultant"))
                            return RedirectToAction("Index", "Consultantf");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                    foreach (var error in result.Errors.Select(e => e.Description))
                        ModelState.AddModelError("", error);
            }
            return View(registerModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var username = User.Identity.Name;

            var user = await userManager.FindByNameAsync(username);
            var model = new UpdateProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,

                IDNumber = user.IDnumber,

                Userrole = user.UserRole,
                 Lastname=user.LastName +" "+user.FirstName,
        
        };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;

                var user = await userManager.FindByNameAsync(username);
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync
                        (user, model.Password, isPersistent: model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsInRoleAsync(user, "Consultant"))
                            return RedirectToAction("Index", "Consultant");
                        return Redirect(model?.ReturnUrl ?? "/Home/Index");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}