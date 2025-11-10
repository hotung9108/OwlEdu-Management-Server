using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;
using System.Linq.Expressions;

namespace OwlEdu_Manager_Server.Services
{
    public class BaseService<T> : IService<T> where T : class
    {
        private readonly EnglishCenterManagementContext _context;
        private readonly DbSet<T> _dbSet;
        public BaseService(EnglishCenterManagementContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize)
        {
            return await _dbSet.Where(predicate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        //public Task<IEnumerable<T>> GetByKeywordAsync(string keyword, int pageNumber, int pageSize)
        //{
        //    var query = _dbSet.AsEnumerable();
        //    var stringProperties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));
        //    var results = query.Where(entity =>
        //    {
        //        foreach (var property in stringProperties)
        //        {
        //            var value = property.GetValue(entity) as string;
        //            if (!string.IsNullOrEmpty(value) && value.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    });

        //    return Task.FromResult<IEnumerable<T>>(results.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
        //}
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
