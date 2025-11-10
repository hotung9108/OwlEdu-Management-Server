using System.Linq.Expressions;

namespace OwlEdu_Manager_Server.Services
{
    public interface IService<T> where T:class
    {
        Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize);
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
        //Task<IEnumerable<T>> GetByKeywordAsync(string keyword, int pageNumber, int pageSize);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(object id);
    }
}
