using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Service;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartWasteCollectionSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly SwcsdbContext _context;
        private readonly EmailService _emailService;

        public LoginController(SwcsdbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(new User()
            {
                MoveInDate = DateTime.Now,
                CreatedDate = DateTime.Now
            });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var pass = ComputeMd5Hash(password);
            var user = _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefault(u => u.Email == username && u.Password == pass);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                    new Claim("Id", user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.UserRole.RoleName) // Update to have different roles
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
                // Successful login
                // You can set session or authentication cookie here
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Invalid credentials
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View("Index");
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterNewAccount(User user)
        {
            user.UserRoleId = _context.UserRoles.FirstOrDefault(r => r.RoleName == "Home Owner").UserRoleId;
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email already exists.";
                return View("Register");
            }
            user.Password = ComputeMd5Hash(user.Password);
            user.CreatedDate = DateTime.Now;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Login");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
        public string ComputeMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
