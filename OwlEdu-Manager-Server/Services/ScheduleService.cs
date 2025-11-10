using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class ScheduleService : BaseService<Schedule>
    {
        public ScheduleService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
