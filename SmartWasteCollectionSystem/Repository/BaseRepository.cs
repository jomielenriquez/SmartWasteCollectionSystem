using Microsoft.EntityFrameworkCore;
using SmartWasteCollectionSystem.Interface;
using SmartWasteCollectionSystem.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartWasteCollectionSystem.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly SwcsdbContext _context;
        private readonly TimeZoneInfo _timeZoneInfo;
        public BaseRepository(SwcsdbContext context)
        {
            _context = context;
            _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
        }
        private void ConvertDateTimesToTimeZone(T data)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.CanWrite)
                {
                    if (prop.PropertyType == typeof(DateTime))
                    {
                        var value = (DateTime)prop.GetValue(data);
                        // Only convert if not default value
                        if (value != default)
                        {
                            // Assume value is UTC, convert to configured time zone
                            prop.SetValue(data, TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc), _timeZoneInfo));
                        }
                    }
                    else if (prop.PropertyType == typeof(DateTime?))
                    {
                        var value = (DateTime?)prop.GetValue(data);
                        if (value.HasValue && value.Value != default)
                        {
                            prop.SetValue(data, TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc), _timeZoneInfo));
                        }
                    }
                }
            }
        }
        public IEnumerable<T> GetAll()
        {
            try
            {
                return _context.Set<T>().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> GetByCondition(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(predicate);

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public T GetById(Guid id)
        {
            try
            {
                var entity = _context.Set<T>().Find(id);
                if (entity == null)
                {
                    return null;
                }
                _context.Entry(entity).State = EntityState.Detached;
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T GetByIntId(int id)
        {
            try
            {
                var entity = _context.Set<T>().Find(id);
                if (entity == null)
                {
                    return null;
                }
                _context.Entry(entity).State = EntityState.Detached;
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T Save(T data)
        {
            try
            {
                ConvertDateTimesToTimeZone(data);
                _context.Set<T>().Add(data);
                _context.SaveChanges();
                return data;
            }
            catch (Exception ex)
            {
                //_context.Entry(data).State = EntityState.Detached;
                //var error = new ErrorLog
                //{
                //    InnerException = ex.InnerException?.Message,
                //    Message = ex.Message,
                //    Source = ex.Source,
                //    StackTrace = ex.StackTrace
                //};
                //_context.ErrorLogs.Add(error);
                //_context.SaveChanges();
                return data;
            }
        }

        public T Update(T data)
        {
            try
            {
                ConvertDateTimesToTimeZone(data);
                _context.Set<T>().Update(data);
                _context.SaveChanges();
                return data;
            }
            catch (Exception ex)
            {
                //_context.Entry(data).State = EntityState.Detached;
                //var error = new ErrorLog
                //{
                //    InnerException = ex.InnerException?.Message,
                //    Message = ex.Message,
                //    Source = ex.Source,
                //    StackTrace = ex.StackTrace
                //};
                //_context.ErrorLogs.Add(error);
                //_context.SaveChanges();
                return data;
            }
        }

        /*
         var pageNumber = 2;
        var pageSize = 10;
        Expression<Func<YourEntityType, bool>> filter = x => x.SomeProperty == "SomeValue";
        var paginatedResults = repository.GetAll(pageNumber, pageSize, filter);

        var paginatedResults = repository.GetAll(pageNumber, pageSize);
         */
        public IEnumerable<T> GetWithOption(int pageNumber, int pageSize, Expression<Func<T, bool>> filter = null)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> GetAllWithOptions(PageModel pageModel, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }


            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            PropertyInfo[] properties = typeof(T).GetProperties();

            // Build the expression tree for dynamic ordering
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression propertyAccess = null;

            PropertyInfo property = properties.FirstOrDefault(p => p.Name == pageModel.OrderByProperty);
            if (property != null)
            {
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }

            // Create the final lambda expression
            if (propertyAccess != null)
            {
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);
                var methodName = pageModel.IsAscending ? "OrderBy" : "OrderByDescending";
                var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExpression));
                query = query.Provider.CreateQuery<T>(resultExpression);
            }

            // Apply pagination
            query = query.Skip((pageModel.Page - 1) * pageModel.PageSize).Take(pageModel.PageSize);

            return query.ToList();
        }

        public IEnumerable<T> GetAllWithOptionsAndIncludes(PageModel pageModel, Expression<Func<T, bool>> filter = null, params string[] includePaths)
        {
            IQueryable<T> query = _context.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includePath in includePaths)
            {
                query = query.Include(includePath);
            }

            PropertyInfo[] properties = typeof(T).GetProperties();

            // Build the expression tree for dynamic ordering
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression propertyAccess = null;

            PropertyInfo property = properties.FirstOrDefault(p => p.Name == pageModel.OrderByProperty);
            if (property != null)
            {
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }

            // Create the final lambda expression
            if (propertyAccess != null)
            {
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);
                var methodName = pageModel.IsAscending ? "OrderBy" : "OrderByDescending";
                var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExpression));
                query = query.Provider.CreateQuery<T>(resultExpression);
            }

            // Apply pagination
            query = query.Skip((pageModel.Page - 1) * pageModel.PageSize).Take(pageModel.PageSize);

            return query.ToList();
        }
        public int GetCountWithOptions(PageModel pageModel, Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _context.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Count();
        }
        public int DeleteWithIds(Guid[] ids, string idName)
        {
            try
            {
                var entitiesToDelete = _context.Set<T>().AsEnumerable().Where(e => ids.Contains((Guid)typeof(T).GetProperty(idName)?.GetValue(e, null)));

                _context.Set<T>().RemoveRange(entitiesToDelete);

                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting entities by IDs.", ex);
            }
        }

        public int DeleteByCondition(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> entititesToDelete = _context.Set<T>().Where(predicate);

                foreach (var include in includes)
                {
                    entititesToDelete = entititesToDelete.Include(include);
                }

                _context.Set<T>().RemoveRange(entititesToDelete);

                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> GetByConditionAndIncludes(Expression<Func<T, bool>> predicate, params string[] includePaths)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(predicate);

                foreach (var includePath in includePaths)
                {
                    query = query.Include(includePath);
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
