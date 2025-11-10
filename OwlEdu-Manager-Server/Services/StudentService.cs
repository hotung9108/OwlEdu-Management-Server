using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class StudentService : BaseService<Student>
    {
        public StudentService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
