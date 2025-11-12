using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class AccountService : BaseService<Account>
    {
        public AccountService(EnglishCenterManagementContext context) : base(context)
        {
        }


    }
}
