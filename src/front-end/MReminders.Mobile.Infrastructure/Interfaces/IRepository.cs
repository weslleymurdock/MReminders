using System.Linq.Expressions;

namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface IRepository <T> where T : class
{
    Task<bool> AddAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter);
    Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter, Expression<Func<T, object>> includes);
    Task<bool> UpdateAsync(T entity);
}
