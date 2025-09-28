using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using System.Security.Claims;

namespace SmartWasteCollectionSystem.Controllers
{
    public class HomeOwnerController : Controller
    {
        private readonly IBaseRepository<MonthlyDue> _monthlyDue;
        private readonly IBaseRepository<Announcement> _announcement;
        public HomeOwnerController(IBaseRepository<MonthlyDue> monthlyDue, IBaseRepository<Announcement> announcement)
        {
            _monthlyDue = monthlyDue;
            _announcement = announcement;
        }
        [Authorize(Roles = "Home Owner")]
        public IActionResult MonthlyDues()
        {
            var result = _monthlyDue.GetByConditionAndIncludes(x => 
                x.UserId == Guid.Parse(User.FindFirstValue("Id")), "User").ToList();
            return View(result);
        }
        [Authorize(Roles = "Home Owner")]
        public IActionResult PaymentHistory()
        {
            var result = _monthlyDue.GetByConditionAndIncludes(x =>
                x.UserId == Guid.Parse(User.FindFirstValue("Id")) 
                && x.IsPaid,
                "User")
                .OrderByDescending(x => x.CreatedDate).ToList();
            return View(result);
        }
        [Authorize(Roles = "Home Owner, Garbage Collector")]
        public IActionResult Announcements()
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila"); // Change to your time zone
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            var result = _announcement.GetByConditionAndIncludes(x =>
                x.IsActive
                && x.StartDate <= DateOnly.FromDateTime(localTime)
                && x.EndDate >= DateOnly.FromDateTime(localTime))
                .OrderByDescending(x => x.CreatedDate).ToList();
            return View(result);
        }
    }
}
