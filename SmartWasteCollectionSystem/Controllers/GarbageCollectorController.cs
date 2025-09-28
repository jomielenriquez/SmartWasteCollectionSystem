using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;

namespace SmartWasteCollectionSystem.Controllers
{
    public class GarbageCollectorController : Controller
    {
        private readonly IBaseRepository<User> _user;
        private readonly IBaseRepository<GarbageCollectionSchedule> _garbageCollectionSchedule;

        public GarbageCollectorController(IBaseRepository<User> user, IBaseRepository<GarbageCollectionSchedule> garbageCollectionSchedule)
        {
            _user = user;
            _garbageCollectionSchedule = garbageCollectionSchedule;
        }
        [Authorize(Roles = "Garbage Collector")]
        public IActionResult RouteMapping()
        {
            var user = _user.GetAll();
            return View(user);
        }
    }
}
