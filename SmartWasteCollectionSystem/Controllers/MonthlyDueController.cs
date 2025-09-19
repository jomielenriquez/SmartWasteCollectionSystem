using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Repository;

namespace SmartWasteCollectionSystem.Controllers
{
    public class MonthlyDueController : Controller
    {
        private readonly IBaseRepository<MonthlyDue> _monthlyDueRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly MonthlyDueRepository _monthlyDue;
        public MonthlyDueController(IBaseRepository<MonthlyDue> monthlyDueRepository, IBaseRepository<User> userRepository, IBaseRepository<UserRole> userRoleRepository)
        {
            _monthlyDueRepository = monthlyDueRepository;
            _userRepository = userRepository;
            _monthlyDue = new MonthlyDueRepository(_monthlyDueRepository);
            _userRoleRepository = userRoleRepository;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListScreen(PageModel pageModel)
        {
            var homeOwners = _userRoleRepository.GetByCondition(UserRole => UserRole.RoleName == "Home Owner").FirstOrDefault();
            var result = _monthlyDue.GetList(pageModel);
            result.Data.User = _userRepository.GetByConditionAndIncludes(x =>
                        x.UserRoleId == homeOwners.UserRoleId
                    ).Select(x => new
                    {
                        UserId = x.UserId,
                        FullName = x.FirstName + " " + x.LastName + " (" + x.Email + ")"
                    }).Cast<object>().ToList();
            return View(result.Data);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult MonthlyDueSearch(MonthlyDue monthlyDueSearchModel)
        {
            var pageModel = new PageModel();
            pageModel.Search = JsonConvert.SerializeObject(monthlyDueSearchModel);

            return RedirectToAction("ListScreen", "MonthlyDue", pageModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public int DeleteMonthlyDue([FromBody] Guid[] selected)
        {
            var result = _monthlyDue.DeleteMonthlyDue(selected);
            return result.DeleteCount;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult MonthlyDueEdit(MonthlyDue monthlyDue)
        {
            var homeOwners = _userRoleRepository.GetByCondition(UserRole => UserRole.RoleName == "Home Owner").FirstOrDefault();
            var result = _monthlyDue.GetMonthlyDueById(monthlyDue.MonthlyDueId);
            var editScreen = new EditScreenModel<MonthlyDue>()
            {
                Data = result.Data,
                Users = _userRepository.GetByConditionAndIncludes(x =>
                        x.UserRoleId == homeOwners.UserRoleId
                    )
                    .Select(x => new
                    {
                        UserId = x.UserId,
                        FullName = x.FirstName + " " + x.LastName + " (" + x.Email + ")"
                    }).Cast<object>().ToList()
            };
            if (monthlyDue.UserId != Guid.Empty)
            {
                return View(editScreen);
            }

            return View(editScreen);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Save(MonthlyDue monthlyDue)
        {
            var result = _monthlyDue.SaveUser(monthlyDue);
            if (!result.IsSuccess)
            {
                var editScreen = new EditScreenModel<MonthlyDue>()
                {
                    Data = result.Data,
                    UserRoles = _userRepository.GetAll().Cast<object>().ToList(),
                    ErrorMessages = result.Errors ?? new List<string> { }
                };
                return View("MonthlyDueEdit", editScreen);
            }

            return RedirectToAction("ListScreen", "MonthlyDue");
        }
    }
}
