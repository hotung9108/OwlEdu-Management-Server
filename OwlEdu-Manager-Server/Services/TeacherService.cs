using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class TeacherService : BaseService<Teacher>
    {
        public TeacherService(EnglishCenterManagementContext context) : base(context)
        {
        }
        public async Task<Teacher?> GetTeacherByAccountIdAsync(string accountId)
        {
            return await _dbSet.FirstOrDefaultAsync(teacher => teacher.AccountId == accountId);
        }
    }
}
