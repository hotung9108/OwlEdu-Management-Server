using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class ScoreService : BaseService<Score>
    {
        public ScoreService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
