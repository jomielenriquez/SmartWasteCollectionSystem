using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SmartWasteCollectionSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly SwcsdbContext _context;
        private readonly IBaseRepository<GarbageCollectionSchedule> _schedule;
        private readonly ILogger<HomeController> _logger;
        private readonly IBaseRepository<User> _user;
        private readonly IBaseRepository<BinLog> _bin;

        public HomeController(
            ILogger<HomeController> logger, 
            SwcsdbContext context,
            IBaseRepository<GarbageCollectionSchedule> schedule,
            IBaseRepository<User> user,
            IBaseRepository<BinLog> bin
        )
        {
            _logger = logger;
            _context = context;
            _schedule = schedule;
            _user = user;
            _bin = bin;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var dashboardData = new DashboardModel();

            if (userRole == "Admin") {
                
                dashboardData.GarbageCollectionSchedules = _schedule.GetByConditionAndIncludes(g =>
                    g.IsActive == true,
                    "DayOfWeek",
                    "FrequencyType"
                ).ToList();

                dashboardData.Users = _user.GetByCondition(u =>
                    u.CreatedDate.Year == DateTime.Now.Year &&
                    u.CreatedDate.Month == DateTime.Now.Month
                ).ToList();
            }
            else if (userRole == "Home Owner")
            {
                dashboardData.BinStatusPercentage = _bin.GetByCondition(b =>
                    b.UserId == Guid.Parse(User.FindFirstValue("Id"))
                ).OrderByDescending(b => b.CreatedDate).Select(b => b.BinStatusPercentage).FirstOrDefault();
            }

                return View(dashboardData);
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
