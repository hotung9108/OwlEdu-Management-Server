using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;

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

            var coursesByString = _courseService.GetByStringKeywordAsync(keyword, pageNumber, pageSize);
            var coursesByNumeric = _courseService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize);

            await Task.WhenAll(coursesByString, coursesByNumeric);

            var res = coursesByString.Result.Concat(coursesByNumeric.Result).DistinctBy(t => t.Id).Select(t => ModelMapUtils.MapBetweenClasses<Course, CourseDTO>(t)).ToList();
            
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(string id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return BadRequest(new {Message = "Course not found."});
            }

            var res = ModelMapUtils.MapBetweenClasses<Course, CourseDTO>(course);

            return Ok(res);  
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CourseDTO courseDTO)
        {
            if (courseDTO == null)
            {
                return BadRequest(new {Message = "Invalid course data."});
            }

            var existingCourse = await _courseService.GetByIdAsync(courseDTO.Id);
            if (existingCourse != null)
            {
                return BadRequest(new { Message = "Course not found." });
            }

            var course = ModelMapUtils.MapBetweenClasses<CourseDTO, Course>(courseDTO);

            await _courseService.AddAsync(course);

            return CreatedAtAction(nameof(GetCourseById), new {id = course.Id}, course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(string id, [FromBody] CourseDTO courseDTO)
        {
            if (id == null || courseDTO == null)
            {
                return BadRequest(new { Message = "Invalid course data." });
            }

            var existingCourse = await _courseService.GetByIdAsync(id);
            if (existingCourse == null)
            {
                return BadRequest(new { Message = "Course not found." });
            }

            var course = ModelMapUtils.MapBetweenClasses<CourseDTO, Course>(courseDTO);

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
