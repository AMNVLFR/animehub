using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AnimeHub.Models;

namespace AnimeHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            var user = new ApplicationUser
            {
                UserName = username,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Redirect("/");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(string userName, string firstName, string lastName, string bio, DateTime? dateOfBirth)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Check if username is changed
            if (user.UserName != userName)
            {
                var existingUser = await _userManager.FindByNameAsync(userName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username is already taken.");
                    return View(user);
                }
                user.UserName = userName;
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Bio = bio;
            user.DateOfBirth = dateOfBirth;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "Profile updated successfully.";
                return RedirectToAction("EditProfile");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(user);
            }
        }
    }
}