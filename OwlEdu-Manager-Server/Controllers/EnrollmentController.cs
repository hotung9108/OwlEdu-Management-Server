using Microsoft.AspNetCore.Mvc;

namespace OwlEdu_Manager_Server.Controllers
{
    public class EnrollmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
