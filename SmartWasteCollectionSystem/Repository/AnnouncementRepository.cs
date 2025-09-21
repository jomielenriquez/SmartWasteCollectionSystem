using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using System.Linq.Expressions;

namespace SmartWasteCollectionSystem.Repository
{
    public class AnnouncementRepository
    {
        private readonly IBaseRepository<Announcement> _announcementRepository;

        public AnnouncementRepository(IBaseRepository<Announcement> announcementRepository)
        {
            _announcementRepository = announcementRepository;
        }

        public Results<Announcement> GetAnnouncementById(Guid announcementId)
        {
            var result = new Results<Announcement>();
            result.IsSuccess = true;
            result.Data = _announcementRepository.GetByCondition(
                x => x.AnnouncementId == announcementId).FirstOrDefault() ?? new Announcement()
                {
                    CreatedDate = DateTime.Now,
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now)
                };
            return result;
        }
        private Results<List<string>> IsValidData(Announcement announcementData)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(announcementData.Title))
            {
                errors.Add("Title is required.");
            }
            if (string.IsNullOrWhiteSpace(announcementData.Message))
            {
                errors.Add("Content is required.");
            }
            if (announcementData.StartDate == null)
            {
                errors.Add("Announcement start date is required.");
            }
            if (announcementData.EndDate == null)
            {
                errors.Add("Announcement end date is required.");
            }
            return new Results<List<string>> { IsSuccess = !errors.Any(), Errors = errors };
        }
        public Results<Announcement> SaveAnnouncement(Announcement announcement)
        {
            var result = new Results<Announcement>();
            result.IsSuccess = true;
            result.Data = announcement;
            result.Errors = new List<string>();
            var validationResult = IsValidData(announcement);
            if (!validationResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.Errors = validationResult.Errors;
                return result;
            }
            if (announcement.AnnouncementId == Guid.Empty)
            {
                result.Data = _announcementRepository.Save(announcement);
            }
            else
            {
                var mdResult = _announcementRepository.GetById(announcement.AnnouncementId);
                announcement.CreatedDate = mdResult.CreatedDate;
                result.Data = _announcementRepository.Update(announcement);
            }
            return result;
        }
        public Results<Announcement> DeleteAnnouncement(Guid[] announcementIds)
        {
            var result = new Results<Announcement>();
            result.IsSuccess = true;
            result.DeleteCount = 0;
            if (announcementIds != null && announcementIds.Length > 0)
            {
                int deletedCount = _announcementRepository.DeleteWithIds(announcementIds, "AnnouncementId");
                result.DeleteCount = deletedCount;
                result.Message = $"{deletedCount} announcement(s) deleted successfully.";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "No announcement selected for deletion.";
            }
            return result;
        }
        public Results<ListScreenModel<AnnouncementSearchModel>> GetList(PageModel pageModel)
        {
            var result = new Results<ListScreenModel<AnnouncementSearchModel>>();
            result.IsSuccess = true;

            var search = !string.IsNullOrEmpty(pageModel.Search) ? JsonConvert.DeserializeObject<AnnouncementSearchModel>(pageModel.Search) : new AnnouncementSearchModel();

            Expression<Func<Announcement, bool>> filter = x =>
                (
                    (string.IsNullOrEmpty(search.Title) || x.Title.Contains(search.Title))
                );

            var listScreen = new ListScreenModel<AnnouncementSearchModel>()
            {
                Data = _announcementRepository.GetAllWithOptionsAndIncludes(
                    pageModel,
                    filter).Cast<object>().ToList(),
                Page = 1,
                PageSize = pageModel.PageSize,
                DataCount = _announcementRepository.GetCountWithOptions(pageModel, filter),
                PageMode = pageModel,
                SearchModel = search
            };
            result.Data = listScreen;
            return result;
        }
    }
}
