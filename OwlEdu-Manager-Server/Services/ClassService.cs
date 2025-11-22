using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class ClassService : BaseService<Class>
    {
        public ClassService(EnglishCenterManagementContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Class>> GetClassesByCourseId(string courseId)
        {
            return await _dbSet.AsNoTracking()
                               .Where(c => c.CourseId == courseId)
                               .ToListAsync();
        }
    }
}
