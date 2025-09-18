using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Repository;

namespace SmartWasteCollectionSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserRole> _roleService;
        private readonly UserRepository _user;
        public UserController(IBaseRepository<User> userRepository, IBaseRepository<UserRole> roleService)
        {
            _userRepository = userRepository;
            _user = new UserRepository(_userRepository);
            _roleService = roleService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListScreen(PageModel pageModel)
        {
            var result = _user.GetList(pageModel);
            return View(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UserSearch(UserSearchModel userSearchModel)
        {
            var pageModel = new PageModel();
            pageModel.Search = JsonConvert.SerializeObject(userSearchModel);

            return RedirectToAction("ListScreen", "User", pageModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public int DeleteUser([FromBody] Guid[] selected)
        {
            var result = _user.DeleteUser(selected);
            return result.DeleteCount;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UserEdit(User user)
        {
            var admissionUserResult = _user.GetUserById(user.UserId);
            var editScreen = new EditScreenModel<User>()
            {
                Data = admissionUserResult.Data,
                UserRoles = _roleService.GetAll().Cast<object>().ToList()
            };
            if (user.UserId != Guid.Empty)
            {
                return View(editScreen);
            }

            return View(editScreen);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Save(User user)
        {
            var result = _user.SaveUser(user);
            if (!result.IsSuccess)
            {
                var editScreen = new EditScreenModel<User>()
                {
                    Data = result.Data,
                    UserRoles = _roleService.GetAll().Cast<object>().ToList(),
                    ErrorMessages = result.Errors ?? new List<string> { }
                };
                return View("UserEdit", editScreen);
            }

            return RedirectToAction("ListScreen", "User");
        }
    }
}
