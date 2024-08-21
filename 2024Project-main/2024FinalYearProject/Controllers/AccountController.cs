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

                IdentityResult result = await userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    var signin_result = await signInManager.PasswordSignInAsync(user, registerModel.Password,
                        isPersistent: false, lockoutOnFailure: false);
                    if (signin_result.Succeeded)
                        return RedirectToAction("Index", "Home");
                }
                else
                    foreach (var error in result.Errors.Select(e => e.Description))
                        ModelState.AddModelError("", error);
            }
            return View(registerModel);
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