using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class EnrollmentService : BaseService<Enrollment>
    {
        public EnrollmentService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
