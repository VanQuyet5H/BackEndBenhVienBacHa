using MANAGECOFFEE.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManagementCoffee.Models;
using MANAGECOFFEE.Common;

namespace MANAGECOFFEE.Controllers
{
    public class LoginController : Controller
    {
        private readonly CoffeeShopContext _context;

        public LoginController(CoffeeShopContext context)
        {
            _context = context;
           
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        // Login action
        public IActionResult Login(string username, string password)
        {
            var user = _context.User.FirstOrDefault(u => u.Username == username);
           

            // If the user is found and the password matches
            if (user != null && user.Password == password)
            {
                // Set the user as logged in
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                  );
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Success, "ĐĂNG NHẬP THÀNH CÔNG");
                // Redirect to the home page
                return RedirectToAction("Index","Home");
            }
            ViewBag.Alert = CommonServices.ShowAlert(Alerts.Warning, "Username or password không hợp lệ");
            // Otherwise, return an error
            return RedirectToAction("Login", new { ErrorMessage = "Invalid username or password." });
        }
        // Logout action
        public IActionResult Logout()
        {
            // Clear the user from the session
            HttpContext.Session.Remove("User");

            // Redirect to the login page
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register([FromBody] RegisterUser user)
        {
           
            // Validate user data
            if (ModelState.IsValid)
            {

                // Check if username already exists
                if (_context.User.Any(u => u.Username == user.Username))
                {
                    // Username already exists
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View("Register", user);
                }

                // Create new user
                var newUser = new User
                {
                    Username = user.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    Role = "User"
                };
                if (!user.Password.Equals(user.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and confirm password do not match.");
                    return View("Register", user);
                }
                _context.User.Add(newUser);
                _context.SaveChanges();

                // Login user after registration
                HttpContext.Session.SetString("User",user.Username); 

                return RedirectToAction("Login", "Login");
            }

            // Return error if validation fails
            return View("Register", user);
        }
        public class RegisterUser
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }
        
    }
}
