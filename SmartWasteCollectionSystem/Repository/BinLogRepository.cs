using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Service;
using System.Linq.Expressions;

namespace SmartWasteCollectionSystem.Repository
{
    public class BinLogRepository
    {
        private readonly IBaseRepository<BinLog> _bin;
        private readonly TimeZoneInfo _timeZoneInfo;
        public BinLogRepository(IBaseRepository<BinLog> bin, IConfiguration configuration)
        {
            _bin = bin;
            _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(configuration["TimeZone"] ?? "UTC");
        }
        public Results<BinLog> GetBinLogById(Guid binLogId)
        {
            var result = new Results<BinLog>();
            result.IsSuccess = true;
            result.Data = _bin.GetByCondition(
                x => x.BinLogId == binLogId).FirstOrDefault() ?? new BinLog()
                {
                    CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo)
                };
            return result;
        }
        private Results<List<string>> IsValidData(BinLog binLog)
        {
            var errors = new List<string>();
            if (binLog.UserId == Guid.Empty)
            {
                errors.Add("Title is required.");
            }
            if (binLog.BinStatusPercentage == null)
            {
                errors.Add("Bin status is required.");
            }
            return new Results<List<string>> { IsSuccess = !errors.Any(), Errors = errors };
        }
        public Results<BinLog> SaveBinLog(BinLog binLog)
        {
            var result = new Results<BinLog>();
            result.IsSuccess = true;
            result.Data = binLog;
            result.Errors = new List<string>();
            var validationResult = IsValidData(binLog);
            if (!validationResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.Errors = validationResult.Errors;
                return result;
            }
            if (binLog.BinLogId == Guid.Empty)
            {
                binLog.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);
                result.Data = _bin.Save(binLog);
            }
            else
            {
                var mdResult = _bin.GetById(binLog.BinLogId);
                result.Data = _bin.Update(binLog);
            }
            return result;
        }
        public Results<BinLog> DeleteBinLog(Guid[] binLogIds)
        {
            var result = new Results<BinLog>();
            result.IsSuccess = true;
            result.DeleteCount = 0;
            if (binLogIds != null && binLogIds.Length > 0)
            {
                int deletedCount = _bin.DeleteWithIds(binLogIds, "BinLogId");
                result.DeleteCount = deletedCount;
                result.Message = $"{deletedCount} bin log(s) deleted successfully.";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "No bin log selected for deletion.";
            }
            return result;
        }
        public Results<ListScreenModel<BinLogSearchModel>> GetList(PageModel pageModel)
        {
            pageModel.IsAscending = false;
            pageModel.OrderByProperty = "CreatedDate";
            var result = new Results<ListScreenModel<BinLogSearchModel>>();
            result.IsSuccess = true;

            var search = !string.IsNullOrEmpty(pageModel.Search) ? JsonConvert.DeserializeObject<BinLogSearchModel>(pageModel.Search) : new BinLogSearchModel();

            Expression<Func<BinLog, bool>> filter = x =>
                (
                    (search.UserId == Guid.Empty || x.UserId == search.UserId)
                );

            var listScreen = new ListScreenModel<BinLogSearchModel>()
            {
                Data = _bin.GetAllWithOptionsAndIncludes(
                    pageModel,
                    filter).Cast<object>().ToList(),
                Page = 1,
                PageSize = pageModel.PageSize,
                DataCount = _bin.GetCountWithOptions(pageModel, filter),
                PageMode = pageModel,
                SearchModel = search
            };
            result.Data = listScreen;
            return result;
        }
    }
}
