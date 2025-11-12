using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassAssignmentController : ControllerBase
    {
        private readonly ClassAssignmentService _classAssignmentService;

        public ClassAssignmentController(ClassAssignmentService classAssignmentService)
        {
            _classAssignmentService = classAssignmentService;
        }
    }
}
