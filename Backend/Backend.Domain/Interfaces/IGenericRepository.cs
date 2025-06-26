using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Backend.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                      Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        // Find entity based on a predicate
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

        Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<object> AddAsync(T entity);
        Task<object> AddRangeAsync(IEnumerable<T> entities);
        Task<object> UpdateAsync(T entity);
        Task<object> UpdateAsync(T entity, bool trackingOff);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
    }
}
