using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class ClassAssignmentService : BaseService<ClassAssignment>
    {
        public ClassAssignmentService(EnglishCenterManagementContext context) : base(context)
        {
        }

        public async Task<ClassAssignment> GetClassAssignmentByClassIdStudentId(string classId, string studentId)
        {
            return await _dbSet.AsNoTracking().Where(t => t.ClassId == classId && t.StudentId == studentId).FirstOrDefaultAsync();
        }

        public async Task DeleteClassAssignment(string classId, string studentId)
        {
            var entity = await GetClassAssignmentByClassIdStudentId(classId, studentId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
