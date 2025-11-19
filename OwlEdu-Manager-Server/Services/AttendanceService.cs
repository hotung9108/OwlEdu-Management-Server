using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class AttendanceService : BaseService<Attendance>
    {
        public AttendanceService(EnglishCenterManagementContext context) : base(context)
        {

        }

        public async Task<Attendance> GetAttendanceByScheduleIdStudentId(string scheduleId, string studentId)
        {
            return await _dbSet.AsNoTracking().Where(attendance => attendance.ScheduleId == scheduleId && attendance.StudentId == studentId).FirstOrDefaultAsync();
        }

        public async Task DeleteAttendance(string scheduleId, string studentId)
        {
            var entity = await _dbSet.FindAsync(studentId, scheduleId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        } 
    }
}
