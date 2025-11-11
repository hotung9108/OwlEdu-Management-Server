using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses([FromQuery] string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (keyword.Trim() == "")
            {
                var courses = await _courseService.GetAllAsync(pageNumber, pageSize);
                return Ok(courses);
            }

            var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var coursesKeyword = await _courseService.FindAsync(c => keywords.Any(k => c.Name == null ? false : c.Name.Contains(k)), pageNumber, pageSize);
            
            return Ok(coursesKeyword);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(string id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return BadRequest(new {Message = "Course not found."});
            }
            return Ok(course);  
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] Course course)
        {
            if (course == null)
            {
                return BadRequest(new {Message = "Invalid course data."});
            }

            await _courseService.AddAsync(course);
            return CreatedAtAction(nameof(GetCourseById), new {id = course.Id}, course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(string id, [FromBody] Course course)
        {
            if (id == null || course == null)
            {
                return BadRequest(new { Message = "Invalid course data." });
            }

            var existingCourse = await _courseService.GetByIdAsync(id);
            if (existingCourse == null)
            {
                return BadRequest(new { Message = "Course not found." });
            }

            await _courseService.UpdateAsync(course);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(string id)
        {
            var existingCourse = _courseService.GetByIdAsync(id);
            if (existingCourse == null)
            {
                return BadRequest(new { Message = "Course not found." });
            }

            await _courseService.DeleteAsync(existingCourse);
            return NoContent();
        }
    }
}
