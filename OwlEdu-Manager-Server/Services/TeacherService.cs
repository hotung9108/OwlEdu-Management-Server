using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class TeacherService : BaseService<Teacher>
    {
        public TeacherService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
