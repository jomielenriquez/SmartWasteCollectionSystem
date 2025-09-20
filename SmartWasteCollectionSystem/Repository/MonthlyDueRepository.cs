using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using System.Linq.Expressions;

namespace SmartWasteCollectionSystem.Repository
{
    public class MonthlyDueRepository
    {
        private readonly IBaseRepository<MonthlyDue> _monthlyDueRepository;
        public MonthlyDueRepository(IBaseRepository<MonthlyDue> monthlyDueRepository)
        {
            _monthlyDueRepository = monthlyDueRepository;
        }

        public Results<MonthlyDue> GetMonthlyDueById(Guid monthlyDueId)
        {
            var result = new Results<MonthlyDue>();
            result.IsSuccess = true;
            result.Data = _monthlyDueRepository.GetByCondition(
                x => x.MonthlyDueId == monthlyDueId,
                x => x.User).FirstOrDefault() ?? new MonthlyDue()
                {
                    DueDate = DateTime.Now,
                    PaidDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    StarDate = DateTime.Now,
                    EndDate = DateTime.Now
                };
            return result;
        }

        public Results<MonthlyDue> SaveUser(MonthlyDue monthlyDue)
        {
            var result = new Results<MonthlyDue>();
            result.IsSuccess = true;
            result.Data = monthlyDue;
            result.Errors = new List<string>();

            var validationResult = IsValidData(monthlyDue);
            if (!validationResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.Errors = validationResult.Errors;
                return result;
            }

            if (monthlyDue.MonthlyDueId == Guid.Empty)
            {
                result.Data = _monthlyDueRepository.Save(monthlyDue);
            }
            else
            {
                var mdResult = _monthlyDueRepository.GetById(monthlyDue.MonthlyDueId);
                monthlyDue.CreatedDate = mdResult.CreatedDate;
                result.Data = _monthlyDueRepository.Update(monthlyDue);
            }

            return result;
        }

        private Results<List<string>> IsValidData(MonthlyDue monthlyDue)
        {
            var errors = new List<string>();
            if (monthlyDue.UserId == Guid.Empty)
            {
                errors.Add("Home owner is required.");
            }
            return new Results<List<string>> { IsSuccess = !errors.Any(), Errors = errors };
        }

        public Results<MonthlyDue> DeleteMonthlyDue(Guid[] monthlyDueIds)
        {
            var result = new Results<MonthlyDue>();
            result.IsSuccess = true;
            result.DeleteCount = 0;
            if (monthlyDueIds != null && monthlyDueIds.Length > 0)
            {
                int deletedCount = _monthlyDueRepository.DeleteWithIds(monthlyDueIds, "MonthlyDueId");
                result.DeleteCount = deletedCount;
                result.Message = $"{deletedCount} payment(s) deleted successfully.";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "No payment selected for deletion.";
            }
            return result;
        }

        public Results<ListScreenModel<MonthlyDueSearchModel>> GetList(PageModel pageModel)
        {
            var result = new Results<ListScreenModel<MonthlyDueSearchModel>>();
            result.IsSuccess = true;

            var search = !string.IsNullOrEmpty(pageModel.Search) ? JsonConvert.DeserializeObject<MonthlyDueSearchModel>(pageModel.Search) : new MonthlyDueSearchModel();

            Expression<Func<MonthlyDue, bool>> filter = x =>
                (
                    (search.UserId == Guid.Empty || x.UserId == search.UserId)
                );

            var listScreen = new ListScreenModel<MonthlyDueSearchModel>()
            {
                Data = _monthlyDueRepository.GetAllWithOptionsAndIncludes(pageModel, filter, "User").Cast<object>().ToList(),
                Page = 1,
                PageSize = pageModel.PageSize,
                DataCount = _monthlyDueRepository.GetCountWithOptions(pageModel, filter),
                PageMode = pageModel,
                SearchModel = search
            };
            result.Data = listScreen;
            return result;
        }
    }
}
