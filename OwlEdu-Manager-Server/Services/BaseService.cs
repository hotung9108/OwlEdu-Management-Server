using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace OwlEdu_Manager_Server.Services
{
    public class BaseService<T> : IService<T> where T : class
    {
        protected readonly EnglishCenterManagementContext _context;
        protected readonly DbSet<T> _dbSet;
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
        public async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize, params string[] orderByProperties)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            // ✅ Nếu có danh sách thuộc tính để sắp xếp
            if (orderByProperties != null && orderByProperties.Length > 0)
            {
                IOrderedQueryable<T>? orderedQuery = null;

                for (int i = 0; i < orderByProperties.Length; i++)
                {
                    string propertyName = orderByProperties[i];
                    var propertyInfo = typeof(T).GetProperty(propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                        continue; // nếu property không tồn tại thì bỏ qua

                    // tạo biểu thức e => e.PropertyName
                    var parameter = Expression.Parameter(typeof(T), "e");
                    var propertyAccess = Expression.Property(parameter, propertyInfo);
                    var lambda = Expression.Lambda(propertyAccess, parameter);

                    // xác định OrderBy hay ThenBy
                    string methodName = (i == 0) ? "OrderBy" : "ThenBy";

                    var method = typeof(Queryable)
                        .GetMethods()
                        .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(typeof(T), propertyInfo.PropertyType);

                    orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, new object[] { orderedQuery ?? query, lambda })!;
                }

                if (orderedQuery != null)
                    query = orderedQuery;
            }

            if (pageNumber != -1)
            {
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }

            return await query.ToListAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetByStringKeywordAsync(string keyword, int pageNumber, int pageSize, params string[] orderByProperties)
        {
            if (string.IsNullOrWhiteSpace(keyword))return Enumerable.Empty<T>();
            var data = await GetAllAsync(-1, -1, orderByProperties);
            var stringProperties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));
            var filtered = data.Where(entity =>
            {
                foreach (var property in stringProperties)
                {
                    var value = property.GetValue(entity) as string;
                    if (!string.IsNullOrEmpty(value) &&
                        value.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            });
            return filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }


        public async Task<IEnumerable<T>> GetByNumericKeywordAsync(string keyword, int pageNumber, int pageSize, params string[] orderByProperties)
        {
            if (string.IsNullOrWhiteSpace(keyword))return Enumerable.Empty<T>();
            if (!decimal.TryParse(keyword, out var numericKeyword))return Enumerable.Empty<T>();
            var data = await GetAllAsync(-1, -1, orderByProperties);
            var numericProperties = typeof(T).GetProperties().Where(p =>
                p.PropertyType == typeof(int) || p.PropertyType == typeof(int?) ||
                p.PropertyType == typeof(double) || p.PropertyType == typeof(double?) ||
                p.PropertyType == typeof(float) || p.PropertyType == typeof(float?) ||
                p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));
            var filtered = data.Where(entity =>
            {
                foreach (var property in numericProperties)
                {
                    var value = property.GetValue(entity);
                    if (value == null) continue;
                    if (value.ToString()!.Contains(keyword) ||
                        Convert.ToDecimal(value) == numericKeyword)
                    {
                        return true;
                    }
                }
                return false;
            });
            return filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        public async Task<IEnumerable<T>> GetByDateTimeKeywordAsync(string keyword, int pageNumber, int pageSize, params string[] orderByProperties)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<T>();

            var data = await GetAllAsync(-1, -1, orderByProperties);

            var dateProperties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));
            bool parsed = DateTime.TryParse(keyword, out var keywordDate);
            var filtered = data.Where(entity =>
            {
                foreach (var property in dateProperties)
                {
                    var value = property.GetValue(entity);
                    if (value == null) continue;
                    var dateValue = (DateTime)value;
                    if (parsed)
                    {
                        if (dateValue.Date == keywordDate.Date)
                            return true;
                        if (dateValue.Month == keywordDate.Month && dateValue.Year == keywordDate.Year)
                            return true;
                        if (dateValue.Year == keywordDate.Year)
                            return true;
                    }
                    else
                    {
                        var str = dateValue.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        if (str.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
                return false;
            });
            return filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        //public Task<IEnumerable<T>> GetByKeywordAsync(string keyword, int pageNumber, int pageSize)
        //{
        //    var query = _dbSet.AsEnumerable();
        //    var stringProperties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));
        //    var dateTimeProperties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(DateTime));
        //    var numericProperties = typeof(T).GetProperties().Where(p =>
        //        p.PropertyType == typeof(int) || p.PropertyType == typeof(int?) ||
        //        p.PropertyType == typeof(double) || p.PropertyType == typeof(double?) ||
        //        p.PropertyType == typeof(float) || p.PropertyType == typeof(float?) ||
        //        p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));
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
        //        foreach (var property in dateTimeProperties)
        //        {
        //            var value = property.GetValue(entity) as DateTime?;
        //            if (value.HasValue)
        //            {
        //                var fullDateTime = value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        //                if (fullDateTime.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
        //                    value.Value.ToString("yyyy-MM-dd").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
        //                    value.Value.ToString("yyyy").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
        //                    value.Value.ToString("MM").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
        //                    value.Value.ToString("dd").Contains(keyword, StringComparison.OrdinalIgnoreCase))
        //                {
        //                    return true;
        //                }
        //            }

        //        }

        //        foreach (var property in numericProperties)
        //        {
        //            var value = property.GetValue(entity);
        //            if (value != null && value.ToString()!.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    });

        //    return Task.FromResult<IEnumerable<T>>(results.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
        //}
        //public async Task<IEnumerable<T>> GetByDateTimeKeywordAsync(string keyword, int pageNumber, int pageSize)
        //{
        //    if (!DateTime.TryParse(keyword, out var dateValue))
        //    {
        //        return Enumerable.Empty<T>();
        //    }

        //    var query = _dbSet.AsQueryable();

        //    var properties = typeof(T).GetProperties()
        //        .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

        //    foreach (var property in properties)
        //    {
        //        query = query.Where(entity =>
        //            EF.Property<DateTime?>(entity, property.Name) != null &&
        //            EF.Property<DateTime?>(entity, property.Name)!.Value.Date == dateValue.Date);
        //    }

        //    return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        //}
        //public async Task<IEnumerable<T>> GetByNumericKeywordAsync(string keyword, int pageNumber, int pageSize)
        //{
        //    if (!decimal.TryParse(keyword, out var numericValue))
        //    {
        //        return Enumerable.Empty<T>();
        //    }

        //    var query = _dbSet.AsQueryable();

        //    var properties = typeof(T).GetProperties()
        //        .Where(p => p.PropertyType == typeof(int) || p.PropertyType == typeof(int?) ||
        //                    p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?) ||
        //                    p.PropertyType == typeof(double) || p.PropertyType == typeof(double?) ||
        //                    p.PropertyType == typeof(float) || p.PropertyType == typeof(float?));

        //    foreach (var property in properties)
        //    {
        //        query = query.Where(entity =>
        //            EF.Property<object>(entity, property.Name) != null &&
        //            EF.Property<object>(entity, property.Name)!.ToString() == numericValue.ToString(CultureInfo.InvariantCulture));
        //    }
        //    return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        //}
        //public async Task<IEnumerable<T>> GetByStringKeywordAsync(string keyword, int pageNumber, int pageSize)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        return Enumerable.Empty<T>();
        //    }

        //    var query = _dbSet.AsQueryable();
        //    var properties = typeof(T).GetProperties()
        //        .Where(p => p.PropertyType == typeof(string));

        //    foreach (var property in properties)
        //    {
        //        query = query.Where(entity =>
        //            EF.Property<string>(entity, property.Name) != null &&
        //            EF.Functions.Like(EF.Property<string>(entity, property.Name), $"%{keyword}%"));
        //    }

        //    return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        //}
    }
}
