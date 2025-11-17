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

        public async Task<IEnumerable<ClassAssignment>> GetClassAssignmentByClassId(string classId)
        {
            return await _dbSet.AsNoTracking().Where(t => t.ClassId == classId).ToListAsync();
        }

        public async Task<IEnumerable<ClassAssignment>> GetClassAssignmentByStudentId(string studentId)
        {
            return await _dbSet.AsNoTracking().Where(t => t.StudentId == studentId).ToListAsync();
        }
    }
}
