using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Repository;

namespace SmartWasteCollectionSystem.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly IBaseRepository<Announcement> _announcement;
        private readonly AnnouncementRepository _announcementRepository;
        
        public AnnouncementController(IBaseRepository<Announcement> announcement)
        {
            _announcement = announcement;
            _announcementRepository = new AnnouncementRepository(announcement);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ListScreen(PageModel pageModel)
        {
            var result = _announcementRepository.GetList(pageModel);
            return View(result.Data);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AnnouncementSearch(Announcement announcementSearchModel)
        {
            var pageModel = new PageModel();
            pageModel.Search = JsonConvert.SerializeObject(announcementSearchModel);

            return RedirectToAction("ListScreen", "Announcement", pageModel);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public int DeleteAnnouncement([FromBody] Guid[] selected)
        {
            var result = _announcementRepository.DeleteAnnouncement(selected);
            return result.DeleteCount;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AnnouncementEdit(Announcement announcement)
        {
            var result = _announcementRepository.GetAnnouncementById(announcement.AnnouncementId);
            var editScreen = new EditScreenModel<Announcement>()
            {
                Data = result.Data
            };
            return View(editScreen);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Save(Announcement announcement)
        {
            var result = _announcementRepository.SaveAnnouncement(announcement);
            if (!result.IsSuccess)
            {
                var editScreen = new EditScreenModel<Announcement>()
                {
                    Data = result.Data,
                    ErrorMessages = result.Errors ?? new List<string> { }
                };
                return View("AnnouncementEdit", editScreen);
            }

            return RedirectToAction("ListScreen", "Announcement");
        }
    }
}
