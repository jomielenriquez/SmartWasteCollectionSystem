using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Repository;

namespace SmartWasteCollectionSystem.Controllers
{
    public class GarbageCollectionScheduleController : Controller
    {
        private readonly GarbageCollectionScheduleRepository _garbageCollectionScheduleRepository;
        private readonly IBaseRepository<Models.GarbageCollectionSchedule> _garbageCollectionScheduleRepo;
        private readonly IBaseRepository<Models.DayOfWeek> _dayOfWeekRepo;
        private readonly IBaseRepository<Models.FrequencyType> _frequencyTypeRepo;
        public GarbageCollectionScheduleController(
            IBaseRepository<Models.GarbageCollectionSchedule> garbageCollectionScheduleRepository,
            IBaseRepository<Models.DayOfWeek> dayOfWeekRepository,
            IBaseRepository<Models.FrequencyType> frequencyTypeRepository)
        {
            _garbageCollectionScheduleRepo = garbageCollectionScheduleRepository;
            _dayOfWeekRepo = dayOfWeekRepository;
            _frequencyTypeRepo = frequencyTypeRepository;
            _garbageCollectionScheduleRepository = new GarbageCollectionScheduleRepository(
                garbageCollectionScheduleRepository,
                dayOfWeekRepository,
                frequencyTypeRepository);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListScreen(PageModel pageModel)
        {
            var result = _garbageCollectionScheduleRepository.GetList(pageModel);
            result.Data.DayOfWeek = _dayOfWeekRepo.GetAll().Cast<object>().ToList();
            result.Data.FrequencyType = _frequencyTypeRepo.GetAll().Cast<object>().ToList();
            return View(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult GarbageCollectionScheduleSearch(GarbageCollectionSchedule garbageCollecitonSchedule)
        {
            var pageModel = new PageModel();
            pageModel.Search = JsonConvert.SerializeObject(garbageCollecitonSchedule);

            return RedirectToAction("ListScreen", "GarbageCollectinSchedule", pageModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public int DeleteGarbageCollectionSchedule([FromBody] Guid[] selected)
        {
            var result = _garbageCollectionScheduleRepository.DeleteGarbageCollectionSchedule(selected);
            return result.DeleteCount;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GarbageCollectionScheduleEdit(GarbageCollectionSchedule gabageCollectionSchedule)
        {
            var result = _garbageCollectionScheduleRepository.GetGarbageCollectionScheduleById(gabageCollectionSchedule.GarbageCollectionScheduleId);
            var editScreen = new EditScreenModel<GarbageCollectionSchedule>()
            {
                Data = result.Data,
                FrequencyType = _frequencyTypeRepo.GetAll().Cast<object>().ToList(),
                DayOfWeek = _dayOfWeekRepo.GetAll().Cast<object>().ToList()
            };
            return View(editScreen);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Save(GarbageCollectionSchedule gabageCollectionSchedule)
        {
            var result = _garbageCollectionScheduleRepository.SaveGarbageCollectionSchedule(gabageCollectionSchedule);
            if (!result.IsSuccess)
            {
                var editScreen = new EditScreenModel<GarbageCollectionSchedule>()
                {
                    Data = result.Data,
                    FrequencyType = _frequencyTypeRepo.GetAll().Cast<object>().ToList(),
                    DayOfWeek = _dayOfWeekRepo.GetAll().Cast<object>().ToList(),
                    ErrorMessages = result.Errors ?? new List<string> { }
                };
                return View("GarbageCollectionScheduleEdit", editScreen);
            }

            return RedirectToAction("ListScreen", "GarbageCollectionSchedule");
        }
    }
}
