using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class PaymentService : BaseService<Payment>
    {
        public PaymentService(EnglishCenterManagementContext context) : base(context)
        {
        }
    }
}
