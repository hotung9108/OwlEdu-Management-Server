using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;

namespace OwlEdu_Manager_Server.Controllers
{
    [Authorize]
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
                var courses = await _courseService.GetAllAsync(pageNumber, pageSize, "Id");
                return Ok(courses.Select(t => ModelMapUtils.MapBetweenClasses<Course, CourseDTO>(t)).ToList());
            }

            var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IEnumerable<Course> finalResult = null;

            foreach (var key in keywords)
            {
                // Tìm theo từng loại
                var byStr = await _courseService.GetByStringKeywordAsync(key, -1, pageSize, "Id");
                var byNum = await _courseService.GetByNumericKeywordAsync(key, -1, pageSize, "Id");

                // Ghép kết quả của TỪ khóa hiện tại
                var unionForCurrentKeyword = byStr
                    .Concat(byNum)
                    .DistinctBy(t => t.Id);

                // Lần đầu → gán luôn
                if (finalResult == null)
                    finalResult = unionForCurrentKeyword;
                else
                    // Giao nhau để chỉ giữ các item match “mọi keyword”
                    finalResult = finalResult.Intersect(unionForCurrentKeyword);
            }

            // Map sang DTO
            var res = finalResult.Select(t => ModelMapUtils.MapBetweenClasses<Course, CourseDTO>(t)).ToList();
            if (pageNumber != -1)
            {
                res = finalResult.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(t => ModelMapUtils.MapBetweenClasses<Course, CourseDTO>(t)).ToList();
            }

            if (res == null) res = new List<CourseDTO>();

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

            var course = ModelMapUtils.MapBetweenClasses<CourseDTO, Course>(courseDTO);

            var courses = await _courseService.GetAllAsync(-1, -1, "Id");

            if (courses.Count() == 0)
            {
                course.Id = "KH000";
            }
            else
            {
                var last = courses.Last();

                int lastestIdNumber = int.Parse(last.Id.Substring(2));

                int newIdNumber = lastestIdNumber + 1;

                string newId = "KH";

                for (int i = 0; i < 3 - newIdNumber.ToString().Length; i++) newId += "0";

                course.Id = newId + newIdNumber.ToString();
            }

            await _courseService.AddAsync(course);

            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, ModelMapUtils.MapBetweenClasses<Course, CourseDTO>(course));
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

            existingCourse.Name = courseDTO.Name;
            existingCourse.Description = courseDTO.Description;
            existingCourse.Status = courseDTO.Status;
            existingCourse.NumberOfLessons = courseDTO.NumberOfLessons;
            existingCourse.Fee = courseDTO.Fee;

            await _courseService.UpdateAsync(existingCourse);
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

            await _courseService.DeleteAsync(id);
            return NoContent();
        }
    }
}
