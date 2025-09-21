using Newtonsoft.Json;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using SmartWasteCollectionSystem.Service;
using System.Linq.Expressions;

namespace SmartWasteCollectionSystem.Repository
{
    public class EmailRepository
    {
        private readonly IBaseRepository<Email> _email;
        private readonly EmailService _emailService;
        public EmailRepository(IBaseRepository<Email> email, EmailService emailService)
        {
            _email = email;
            _emailService = emailService;
        }
        public Results<Email> GetEmailById(Guid emailId)
        {
            var result = new Results<Email>();
            result.IsSuccess = true;
            result.Data = _email.GetByCondition(
                x => x.EmailId == emailId).FirstOrDefault() ?? new Email()
                {
                    CreatedDate = DateTime.Now,
                    SentDate = DateTime.Now,
                };
            return result;
        }
        private Results<List<string>> IsValidData(Email emailData)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(emailData.Title))
            {
                errors.Add("Title is required.");
            }
            if (string.IsNullOrWhiteSpace(emailData.Message))
            {
                errors.Add("Message is required.");
            }
            if (string.IsNullOrWhiteSpace(emailData.Recipients))
            {
                errors.Add("Recipient(s) is required.");
            }
            return new Results<List<string>> { IsSuccess = !errors.Any(), Errors = errors };
        }
        public Results<Email> SaveEmail(Email email)
        {
            var result = new Results<Email>();
            result.IsSuccess = true;
            result.Data = email;
            result.Errors = new List<string>();
            var validationResult = IsValidData(email);
            if (!validationResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.Errors = validationResult.Errors;
                return result;
            }
            if (email.IsSent)
            {
                email.SentDate = DateTime.Now;
                _emailService.SendEmail(email.Recipients, email.Title, email.Message);
            }
            if (email.EmailId == Guid.Empty)
            {
                result.Data = _email.Save(email);
            }
            else
            {
                var mdResult = _email.GetById(email.EmailId);
                email.CreatedDate = mdResult.CreatedDate;
                result.Data = _email.Update(email);
            }
            return result;
        }
        public Results<Email> DeleteAnnouncement(Guid[] emailIds)
        {
            var result = new Results<Email>();
            result.IsSuccess = true;
            result.DeleteCount = 0;
            if (emailIds != null && emailIds.Length > 0)
            {
                int deletedCount = _email.DeleteWithIds(emailIds, "EmailId");
                result.DeleteCount = deletedCount;
                result.Message = $"{deletedCount} email(s) deleted successfully.";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "No email selected for deletion.";
            }
            return result;
        }
        public Results<ListScreenModel<EmailSearchModel>> GetList(PageModel pageModel)
        {
            var result = new Results<ListScreenModel<EmailSearchModel>>();
            result.IsSuccess = true;

            var search = !string.IsNullOrEmpty(pageModel.Search) ? JsonConvert.DeserializeObject<EmailSearchModel>(pageModel.Search) : new EmailSearchModel();

            Expression<Func<Email, bool>> filter = x =>
                (
                    (string.IsNullOrEmpty(search.Title) || x.Title.Contains(search.Title))
                );

            var listScreen = new ListScreenModel<EmailSearchModel>()
            {
                Data = _email.GetAllWithOptionsAndIncludes(
                    pageModel,
                    filter).Cast<object>().ToList(),
                Page = 1,
                PageSize = pageModel.PageSize,
                DataCount = _email.GetCountWithOptions(pageModel, filter),
                PageMode = pageModel,
                SearchModel = search
            };
            result.Data = listScreen;
            return result;
        }
    }
}
