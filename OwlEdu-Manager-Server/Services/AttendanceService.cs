using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class AttendanceService : BaseService<Attendance>
    {
        public AttendanceService(EnglishCenterManagementContext context) : base(context)
        {

        }
    }
}
