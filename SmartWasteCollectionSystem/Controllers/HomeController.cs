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
        private readonly IBaseRepository<MonthlyDue> _due;
        private readonly IBaseRepository<GarbageCollectionSchedule> _garbage;
        private readonly IBaseRepository<Announcement> _annnouncement;

        public HomeController(
            ILogger<HomeController> logger,
            SwcsdbContext context,
            IBaseRepository<GarbageCollectionSchedule> schedule,
            IBaseRepository<User> user,
            IBaseRepository<BinLog> bin,
            IBaseRepository<MonthlyDue> due,
            IBaseRepository<GarbageCollectionSchedule> garbage,
            IBaseRepository<Announcement> annnouncement
        )
        {
            _logger = logger;
            _context = context;
            _schedule = schedule;
            _user = user;
            _bin = bin;
            _due = due;
            _garbage = garbage;
            _annnouncement = annnouncement;
        }

        [Authorize]
        public IActionResult Index()
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila"); // Change to your time zone
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var dashboardData = new DashboardModel();

            if (userRole == "Admin") {

                dashboardData.GarbageCollectionSchedules = _schedule.GetByConditionAndIncludes(g =>
                    g.IsActive == true,
                    "DayOfWeek",
                    "FrequencyType"
                ).ToList();

                dashboardData.Users = _user.GetByCondition(u =>
                    u.CreatedDate.Year == localTime.Year &&
                    u.CreatedDate.Month == localTime.Month
                ).ToList();
            }
            else if (userRole == "Home Owner" || userRole == "Garbage Collector")
            {
                dashboardData.CollectionSchedule = _garbage.GetByConditionAndIncludes(g =>
                    g.IsActive == true
                    && g.EffectiveFrom <= DateOnly.FromDateTime(localTime)
                    && g.EffectiveTo >= DateOnly.FromDateTime(localTime),
                    "DayOfWeek",
                    "FrequencyType"
                ).OrderBy(g => g.DayOfWeek.DayOfWeekId).FirstOrDefault();

                dashboardData.NumberOfAnnouncement = _annnouncement.GetByCondition(a =>
                    a.IsActive == true
                    && a.StartDate <= DateOnly.FromDateTime(localTime)
                    && a.EndDate >= DateOnly.FromDateTime(localTime)
                ).Count();
            }
            else if (userRole == "Home Owner")
            {
                dashboardData.BinStatusPercentage = _bin.GetByCondition(b =>
                    b.UserId == Guid.Parse(User.FindFirstValue("Id"))
                ).OrderByDescending(b => b.CreatedDate).Select(b => b.BinStatusPercentage).FirstOrDefault();

                dashboardData.MonthlyDue = _due.GetByCondition(d =>
                    d.UserId == Guid.Parse(User.FindFirstValue("Id")) &&
                    d.DueDate.Month == localTime.Month &&
                    d.DueDate.Year == localTime.Year &&
                    !d.IsPaid
                ).FirstOrDefault() ?? new MonthlyDue();
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
