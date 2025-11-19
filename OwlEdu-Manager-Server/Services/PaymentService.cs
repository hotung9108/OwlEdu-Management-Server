using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class PaymentService : BaseService<Payment>
    {
        public PaymentService(EnglishCenterManagementContext context) : base(context)
        {
        }
        public async Task<Payment> GetPayementByEnrollmentId(string enrollmentId)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);
        }
    }
}
