using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using System.Security.Claims;

namespace SmartWasteCollectionSystem.Controllers
{
    public class HomeOwnerController : Controller
    {
        private readonly IBaseRepository<MonthlyDue> monthlyDue;
        public HomeOwnerController(IBaseRepository<MonthlyDue> monthlyDue)
        {
            this.monthlyDue = monthlyDue;
        }
        [Authorize(Roles = "Home Owner")]
        public IActionResult MonthlyDues()
        {
            var result = monthlyDue.GetByConditionAndIncludes(x => 
                x.UserId == Guid.Parse(User.FindFirstValue("Id")), "User").ToList();
            return View(result);
        }
        [Authorize(Roles = "Home Owner")]
        public IActionResult PaymentHistory()
        {
            var result = monthlyDue.GetByConditionAndIncludes(x =>
                x.UserId == Guid.Parse(User.FindFirstValue("Id")) 
                && x.IsPaid,
                "User")
                .OrderByDescending(x => x.CreatedDate).ToList();
            return View(result);
        }
    }
}
