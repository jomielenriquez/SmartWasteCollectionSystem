using SmartWasteCollectionSystem.Models;
using System.Linq.Expressions;

namespace SmartWasteCollectionSystem.Interface
{
    public interface IBaseRepository<T>
    {
        IEnumerable<T> GetAll();
        T Save(T data);
        T Update(T data);
        T GetById(Guid id);
        T GetByIntId(int id);
        IEnumerable<T> GetByCondition(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetByConditionAndIncludes(
            Expression<Func<T, bool>> predicate,
            params string[] includePaths);
        int DeleteByCondition(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetWithOption(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null);
        IEnumerable<T> GetAllWithOptions(PageModel pageModel, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAllWithOptionsAndIncludes(PageModel pageModel, Expression<Func<T, bool>> filter = null, params string[] includePaths);
        int GetCountWithOptions(PageModel pageModel, Expression<Func<T, bool>> filter = null);
        int DeleteWithIds(Guid[] ids, string idName);
    }
}
