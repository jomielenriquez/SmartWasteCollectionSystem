using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartWasteCollectionSystem.Models;

namespace SmartWasteCollectionSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly SwcsdbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SwcsdbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
