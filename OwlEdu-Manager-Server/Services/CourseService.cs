using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class CourseService : BaseService<CourseService>
    {
        public CourseService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
