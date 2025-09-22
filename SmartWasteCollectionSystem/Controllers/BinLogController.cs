using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Repository;

namespace SmartWasteCollectionSystem.Controllers
{
    public class BinLogController : Controller
    {
        private readonly IBaseRepository<BinLog> _bin;
        private readonly BinLogRepository _binRepository;
        private readonly IBaseRepository<User> _user;
        private readonly IBaseRepository<UserRole> _role;
        public BinLogController(IBaseRepository<BinLog> bin, IBaseRepository<User> user, IBaseRepository<UserRole> role)
        {
            _bin = bin;
            _user = user;
            _binRepository = new BinLogRepository(bin);
            _role = role;
        }
        [Authorize(Roles = "Home Owner")]
        public IActionResult BinLogListScreen(PageModel pageModel)
        {
            var homeOwners = _role.GetByCondition(UserRole => UserRole.RoleName == "Home Owner").FirstOrDefault();
            var result = _binRepository.GetList(pageModel);
            result.Data.User = _user.GetByConditionAndIncludes(x =>
                        x.UserRoleId == homeOwners.UserRoleId
                    ).Select(x => new
                    {
                        UserId = x.UserId,
                        FullName = x.FirstName + " " + x.LastName + " (" + x.Email + ")"
                    }).Cast<object>().ToList();
            return View(result.Data);
        }
        [Authorize(Roles = "Home Owner")]
        [HttpPost]
        public IActionResult BinLogSearch(BinLog binLogSearchModel)
        {
            var pageModel = new PageModel();
            pageModel.Search = JsonConvert.SerializeObject(binLogSearchModel);

            return RedirectToAction("BinLogListScreen", "BinLog", pageModel);
        }
        [Authorize(Roles = "Home Owner")]
        [HttpPost, ActionName("Delete")]
        public int DeleteBinLog([FromBody] Guid[] selected)
        {
            var result = _binRepository.DeleteBinLog(selected);
            return result.DeleteCount;
        }
        [Authorize(Roles = "Admin, Home Owner")]
        public IActionResult BinLogEdit(BinLog binLog)
        {
            var result = _binRepository.GetBinLogById(binLog.BinLogId);
            var editScreen = new EditScreenModel<BinLog>()
            {
                Data = result.Data,
                ListOfUsers = _user.GetAll().ToList(),
            };
            return View(editScreen);
        }

        [HttpPost, ActionName("UpdateBinStatus")]
        public ActionResult Save(string apiKey, int percentage)
        {
            var user = _user.GetByCondition(u => u.HomeOwnerApikey == new Guid(apiKey)).FirstOrDefault();
            if (user == null) {
                return NotFound(new { message = "User not foud with the provided API key" });
            }
            try
            {
                var binLog = new BinLog()
                {
                    UserId = user.UserId,
                    BinStatusPercentage = percentage
                };
                var result = _binRepository.SaveBinLog(binLog);
                if (result.IsSuccess)
                {
                    return Ok(new { message = "Bin status updated successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating bin status.", error = ex.Message });
            }
            return StatusCode(500, new { message = "An error occurred while updating bin status."});
        }
    }
}
