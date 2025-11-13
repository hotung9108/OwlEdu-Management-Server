using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class StudentService : BaseService<Student>
    {
        public StudentService(EnglishCenterManagementContext context) : base(context)
        {
        }
        public async Task<Student?> GetStudentByAccountIdAsync(string accountId)
        {
            return await _dbSet.FirstOrDefaultAsync(student => student.AccountId == accountId);
        }
    }
}
