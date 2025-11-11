using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : Controller
    {
        private readonly CourseService courseService;

        public CourseController(CourseService _courseService)
        {
            courseService = _courseService;
        }

        //[HttpGet]
        //public Task<IActionResult> GetÁllCourses([FromQuery] int pageNumber, [FromQuery] int pageSize) 
        //{

        //}
    }
}
