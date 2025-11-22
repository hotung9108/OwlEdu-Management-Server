using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class ScheduleService : BaseService<Schedule>
    {
        public ScheduleService(EnglishCenterManagementContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Schedule>> GetSchedulesByClassIdAsync(string classId)
        {
            return await _dbSet.Where(schedule => schedule.ClassId == classId).ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByTeacherIdAsync(string teacherId)
        {
            return await _dbSet.Where(schedule => schedule.TeacherId == teacherId).ToListAsync();
        }
        public async Task<string?> GetMaxScheduleIdAsync()
        {
            return await _dbSet.Where(schedule => schedule.Id.StartsWith("BH")).OrderByDescending(schedule => schedule.Id).Select(schedule => schedule.Id).FirstOrDefaultAsync();
        }
    }
}
