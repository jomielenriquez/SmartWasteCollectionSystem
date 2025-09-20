using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using System.Linq.Expressions;

namespace SmartWasteCollectionSystem.Repository
{
    public class GarbageCollectionScheduleRepository
    {
        private readonly IBaseRepository<GarbageCollectionSchedule> _garbageCollectionScheduleRepository;
        private readonly IBaseRepository<SmartWasteCollectionSystem.Models.DayOfWeek> _dayOfWeekRepository;
        private readonly IBaseRepository<FrequencyType> _frequencyTypeRepository;

        public GarbageCollectionScheduleRepository(
            IBaseRepository<GarbageCollectionSchedule> garbageCollectionScheduleRepository,
            IBaseRepository<Models.DayOfWeek> dayOfWeekRepository, 
            IBaseRepository<FrequencyType> frequencyTypeRepository)
        {
            _garbageCollectionScheduleRepository = garbageCollectionScheduleRepository;
            _dayOfWeekRepository = dayOfWeekRepository;
            _frequencyTypeRepository = frequencyTypeRepository;
        }
        public Results<GarbageCollectionSchedule> GetGarbageCollectionScheduleById(Guid garbageCollectionScheduleId)
        {
            var result = new Results<GarbageCollectionSchedule>();
            result.IsSuccess = true;
            result.Data = _garbageCollectionScheduleRepository.GetByCondition(
                x => x.GarbageCollectionScheduleId == garbageCollectionScheduleId,
                x => x.DayOfWeek,
                x => x.FrequencyType).FirstOrDefault() ?? new GarbageCollectionSchedule()
                {
                    CreatedDate = DateTime.Now
                };
            return result;
        }
        private Results<List<string>> IsValidData(GarbageCollectionSchedule scheduleData)
        {
            var errors = new List<string>();
            if (scheduleData.DayOfWeekId == Guid.Empty)
            {
                errors.Add("Day of week is required.");
            }
            if (scheduleData.FrequencyTypeId == Guid.Empty)
            {
                errors.Add("Frequency type is required.");
            }
            if (scheduleData.CollectionTime == null)
            {
                errors.Add("Collection time is required.");
            }
            if (scheduleData.EffectiveFrom == null)
            {
                errors.Add("Effective from is required.");
            }
            if (scheduleData.EffectiveTo == null)
            {
                errors.Add("Effective to is required.");
            }
            return new Results<List<string>> { IsSuccess = !errors.Any(), Errors = errors };
        }

        public Results<GarbageCollectionSchedule> SaveGarbageCollectionSchedule(GarbageCollectionSchedule garbageCollectionSchedule)
        {
            var result = new Results<GarbageCollectionSchedule>();
            result.IsSuccess = true;
            result.Data = garbageCollectionSchedule;
            result.Errors = new List<string>();

            var validationResult = IsValidData(garbageCollectionSchedule);
            if (!validationResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.Errors = validationResult.Errors;
                return result;
            }

            if (garbageCollectionSchedule.GarbageCollectionScheduleId == Guid.Empty)
            {
                garbageCollectionSchedule.CreatedDate = DateTime.Now;
                result.Data = _garbageCollectionScheduleRepository.Save(garbageCollectionSchedule);
            }
            else
            {
                var mdResult = _garbageCollectionScheduleRepository.GetById(garbageCollectionSchedule.GarbageCollectionScheduleId);
                garbageCollectionSchedule.CreatedDate = mdResult.CreatedDate;
                result.Data = _garbageCollectionScheduleRepository.Update(garbageCollectionSchedule);
            }

            return result;
        }

        public Results<GarbageCollectionSchedule> DeleteGarbageCollectionSchedule(Guid[] garbageCollectionScheduleIds)
        {
            var result = new Results<GarbageCollectionSchedule>();
            result.IsSuccess = true;
            result.DeleteCount = 0;
            if (garbageCollectionScheduleIds != null && garbageCollectionScheduleIds.Length > 0)
            {
                int deletedCount = _garbageCollectionScheduleRepository.DeleteWithIds(garbageCollectionScheduleIds, "GarbageCollectionScheduleId");
                result.DeleteCount = deletedCount;
                result.Message = $"{deletedCount} schedule(s) deleted successfully.";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "No schedule selected for deletion.";
            }
            return result;
        }

        public Results<ListScreenModel<GarbageCollectionScheduleSearchModel>> GetList(PageModel pageModel)
        {
            var result = new Results<ListScreenModel<GarbageCollectionScheduleSearchModel>>();
            result.IsSuccess = true;

            var search = !string.IsNullOrEmpty(pageModel.Search) ? JsonConvert.DeserializeObject<GarbageCollectionScheduleSearchModel>(pageModel.Search) : new GarbageCollectionScheduleSearchModel();

            Expression<Func<GarbageCollectionSchedule, bool>> filter = x =>
                (
                    (search.DayOfWeekId == Guid.Empty || x.DayOfWeekId == search.DayOfWeekId)
                    && (search.FrequencyTypeId == Guid.Empty || x.FrequencyTypeId == search.FrequencyTypeId)
                );

            var listScreen = new ListScreenModel<GarbageCollectionScheduleSearchModel>()
            {
                Data = _garbageCollectionScheduleRepository.GetAllWithOptionsAndIncludes(
                    pageModel, 
                    filter, 
                    "DayOfWeek",
                    "FrequencyType").Cast<object>().ToList(),
                Page = 1,
                PageSize = pageModel.PageSize,
                DataCount = _garbageCollectionScheduleRepository.GetCountWithOptions(pageModel, filter),
                PageMode = pageModel,
                SearchModel = search
            };
            result.Data = listScreen;
            return result;
        }
    }
}
