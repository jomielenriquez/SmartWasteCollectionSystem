using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Repository;
using SmartWasteCollectionSystem.Service;
using System.Security.Claims;

namespace SmartWasteCollectionSystem.Controllers
{
    public class BinLogController : Controller
    {
        private readonly IBaseRepository<BinLog> _bin;
        private readonly BinLogRepository _binRepository;
        private readonly IBaseRepository<User> _user;
        private readonly IBaseRepository<UserRole> _role;
        private readonly EmailService _emailService;
        private readonly EmailRepository emailRepository;
        public BinLogController(
            IBaseRepository<BinLog> bin, 
            IBaseRepository<User> user, 
            IBaseRepository<UserRole> role,
            EmailService emailService,
            IBaseRepository<Email> email)
        {
            _bin = bin;
            _user = user;
            _binRepository = new BinLogRepository(bin);
            _role = role;
            _emailService = emailService;
            emailRepository = new EmailRepository(email, emailService);
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
        public ActionResult Save(string apiKey, int percentage, int mq3)
        {
            var user = _user.GetByCondition(u => u.HomeOwnerApikey == new Guid(apiKey)).FirstOrDefault();
            var garbageCollectorRoleId = _role.GetByCondition(r => r.RoleName == "Garbage Collector").FirstOrDefault().UserRoleId;
            var collector  = _user.GetByCondition(u => 
                u.UserRoleId == garbageCollectorRoleId
            );
            var garbageCollectors = string.Join(";", collector.Select(c => c.Email));

            if (user == null) {
                return NotFound(new { message = "User not foud with the provided API key" });
            }
            try
            {
                var binLog = new BinLog()
                {
                    UserId = user.UserId,
                    BinStatusPercentage = percentage,
                    Mq3reading = mq3
                };
                var result = _binRepository.SaveBinLog(binLog);

                if (percentage >= 80 || mq3 > 2000)
                {
                    var Location = $"Blk{user.BlockNumber} Lot{user.LotNumber} {user.StreetName}";
                    var FillLevel = percentage;
                    var DashboardUrl = "https://ecokonek.somee.com/";
                    var Mq3Level = mq3;
                    // Send email notification to the user
                    var emailSubject = "Garbage Bin Full – Collection Request";
                    var emailBody = $"\r\n<body style=\"font-family: Arial, Helvetica, sans-serif; margin:0; padding:0; background:#f4f4f4;\">\r\n  <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\">\r\n    <tr>\r\n      <td align=\"center\" style=\"padding:20px 10px;\">\r\n        <table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#ffffff; border-radius:8px; overflow:hidden; box-shadow:0 2px 6px rgba(0,0,0,0.08);\">\r\n          <tr>\r\n            <td style=\"padding:20px; text-align:left;\">\r\n              <!-- Header -->\r\n              <h2 style=\"margin:0 0 8px 0; font-size:20px; color:#333333;\">Garbage Bin Alert — Collection Required</h2>\r\n              <p style=\"margin:0 0 16px 0; color:#555555; font-size:14px;\">\r\n                Hello,\r\n              </p>\r\n\r\n              <!-- Details card -->\r\n              <table width=\"100%\" cellpadding=\"8\" cellspacing=\"0\" role=\"presentation\" style=\"border:1px solid #e8e8e8; border-radius:6px; background:#fafafa;\">\r\n                <tr>\r\n                  <td style=\"font-size:13px; color:#333333; width:45%;\"><strong>Location</strong></td>\r\n                  <td style=\"font-size:13px; color:#333333;\">{Location}</td>\r\n                </tr>\r\n                <tr>\r\n                  <td style=\"font-size:13px; color:#333333;\"><strong>Fill Level</strong></td>\r\n                  <td style=\"font-size:13px; color:#333333;\">{FillLevel}%</td>\r\n                </tr>\r\n                <tr>\r\n                  <td style=\"font-size:13px; color:#333333;\"><strong>MQ135 Level</strong></td>\r\n                  <td style=\"font-size:13px; color:#333333;\">{Mq3Level}</td>\r\n                </tr>\r\n              </table>\r\n\r\n              <!-- CTA -->\r\n              <div style=\"margin:18px 0;\">\r\n                <a href=\"{DashboardUrl}\" style=\"display:inline-block; text-decoration:none; padding:10px 18px; border-radius:6px; background:#007bff; color:#ffffff; font-weight:600; font-size:14px;\">\r\n                  View on Dashboard\r\n                </a>\r\n              </div>\r\n\r\n              <p style=\"margin:0 0 12px 0; color:#555555; font-size:13px;\">\r\n                This is an automated notification from the <strong>Smart Waste Collection System</strong>. Please arrange collection as soon as possible to avoid overflow.\r\n              </p>\r\n\r\n              <!-- Footer -->\r\n              <hr style=\"border:none; border-top:1px solid #eeeeee; margin:18px 0;\">\r\n              <p style=\"margin:0; font-size:12px; color:#999999;\">\r\n                Smart Waste Collection System — Automated Alert<br>\r\n              </p>\r\n            </td>\r\n          </tr>\r\n        </table>\r\n      </td>\r\n    </tr>\r\n  </table>\r\n</body>\r\n";
                    //_emailService.SendEmail(garbageCollectors, emailSubject, emailBody);
                    emailRepository.SaveEmail(new Email
                    {
                        Title = emailSubject,
                        Message = emailBody,
                        Recipients = garbageCollectors,
                        IsSent = true,
                        CreatedDate = DateTime.Now
                    });
                }

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
        [HttpGet]
        public IActionResult GetBinStatus()
        {
            var bin = _bin.GetByCondition(b => 
                b.UserId == Guid.Parse(User.FindFirstValue("Id"))
            ).OrderByDescending(b => b.CreatedDate).Select(b => new 
            {
                b.BinStatusPercentage,
                b.CreatedDate,
                b.Mq3reading
            }).FirstOrDefault();

            return Ok(bin);
        }

    }
}
