using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;

namespace SmartWasteCollectionSystem.Controllers
{
    public class GarbageCollectorController : Controller
    {
        private readonly IBaseRepository<User> _user;
        private readonly IBaseRepository<GarbageCollectionSchedule> _garbageCollectionSchedule;
        private readonly SwcsdbContext _context;

        public GarbageCollectorController(
            IBaseRepository<User> user, 
            IBaseRepository<GarbageCollectionSchedule> garbageCollectionSchedule,
            SwcsdbContext context)
        {
            _user = user;
            _garbageCollectionSchedule = garbageCollectionSchedule;
            _context = context;
        }
        [Authorize(Roles = "Garbage Collector")]
        public IActionResult RouteMapping()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            var paidUsers = _context.Users
                .Where(u =>
                    u.MonthlyDues.Any(md => 
                        md.IsPaid && 
                        md.StarDate.Year == currentDateTime.Year && 
                        md.EndDate.Month == currentDateTime.Month)
                    ).ToList();
            return View(paidUsers);
        }
    }
}
