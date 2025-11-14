using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class ScoreService : BaseService<Score>
    {
        public ScoreService(EnglishCenterManagementContext context) : base(context)
        {
        }
        // Lấy điểm theo lớp
        public async Task<IEnumerable<Score>> GetScoresByClassAsync(string classId)
        {
            return await _dbSet
                .Where(score => score.ClassId == classId)
                .ToListAsync();
        }

        // Lấy điểm theo học viên
        public async Task<IEnumerable<Score>> GetScoresByStudentAsync(string studentId)
        {
            return await _dbSet
                .Where(score => score.StudentId == studentId)
                .ToListAsync();
        }

        // Lấy điểm theo giáo viên
        public async Task<IEnumerable<Score>> GetScoresByTeacherAsync(string teacherId)
        {
            return await _dbSet
                .Where(score => score.TeacherId == teacherId)
                .ToListAsync();
        }
    }
}
