using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class ClassService : BaseService<Class>
    {
        public ClassService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
