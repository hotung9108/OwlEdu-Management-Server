using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class EnrollmentService : BaseService<Enrollment>
    {
        public EnrollmentService(EnglishCenterManagementContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Enrollment>> GetEnrollmentByStudentId(string studentId)
        {
            return await _dbSet.AsNoTracking().Where(e => e.StudentId == studentId).ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentByCourseId(string courseId)
        {
            return await _dbSet.AsNoTracking().Where(e => e.CourseId == courseId).ToListAsync();
        }

        public async Task<Enrollment?> GetEnrollmentByStudentIdCourseId(string studentId, string courseId)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }
    }
}
