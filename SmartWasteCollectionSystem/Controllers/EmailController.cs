using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Repository;
using SmartWasteCollectionSystem.Service;

namespace SmartWasteCollectionSystem.Controllers
{
    public class EmailController : Controller
    {
        private readonly IBaseRepository<Email> _email;
        private readonly EmailRepository _emailRepository;
        private readonly IBaseRepository<User> _userRepository;
        public EmailController(IBaseRepository<Email> email, EmailService emailService, IBaseRepository<User> userRepository)
        {
            _email = email;
            _emailRepository = new EmailRepository(email, emailService);
            _userRepository = userRepository;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EmailListScreen(PageModel pageModel)
        {
            var result = _emailRepository.GetList(pageModel);
            return View(result.Data);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EmailSearch(Email emailSearchModel)
        {
            var pageModel = new PageModel();
            pageModel.Search = JsonConvert.SerializeObject(emailSearchModel);

            return RedirectToAction("EmailListScreen", "Email", pageModel);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public int DeleteAnnouncement([FromBody] Guid[] selected)
        {
            var result = _emailRepository.DeleteAnnouncement(selected);
            return result.DeleteCount;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EmailEdit(Email email)
        {
            var result = _emailRepository.GetEmailById(email.EmailId);
            var editScreen = new EditScreenModel<Email>()
            {
                Data = result.Data,
                ListOfUsers = _userRepository.GetAll().ToList(),
            };
            return View(editScreen);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Save(Email email)
        {
            var result = _emailRepository.SaveEmail(email);
            if (!result.IsSuccess)
            {
                var editScreen = new EditScreenModel<Email>()
                {
                    Data = result.Data,
                    ListOfUsers = _userRepository.GetAll().ToList(),
                    ErrorMessages = result.Errors ?? new List<string> { }
                };
                return View("EmailEdit", editScreen);
            }

            return RedirectToAction("EmailListScreen", "Email");
        }
    }
}
