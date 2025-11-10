using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class ClassAssignmentService : BaseService<ClassAssignment>
    {
        public ClassAssignmentService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
