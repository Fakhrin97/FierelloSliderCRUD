using Fiorello.Models;
using Fiorello.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace Fiorello.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(LogIn));
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existUser = await _userManager.FindByNameAsync(model.Username);

                if(existUser == null)
                {
                    ModelState.AddModelError("", "This User Not Exist");
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(existUser, model.Password, false, true);

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Email tesdiqlenmelidir");
                    return View();
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "This Account Locked Out");
                    return View();
                }

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Password or Username Incorrect");
                    return View();
                }

                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var existUser = await _userManager.FindByNameAsync(model.Username);

            if (existUser != null)
            {
                ModelState.AddModelError("", "Username Artiq Istifade Olunub");
                return View();
            }

            var user = new User
            {
                Fullname = model.Username,
                UserName = model.Username,
                Email = model.Email,
            };

            //var role = await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });

            var result = await _userManager.CreateAsync(user, model.Password);

            //await _userManager.AddToRoleAsync(user , "Admin");

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(LogIn));
        }
    }
}
