using Microsoft.AspNetCore.Mvc;
using SmartWasteCollectionSystem.Interface;
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
    }
}
