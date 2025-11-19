using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;

namespace OwlEdu_Manager_Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentController (EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments(string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (keyword.Trim() == "")
            {
                var enrollments = await _enrollmentService.GetAllAsync(pageNumber, pageSize, "Id");
                return Ok(enrollments);
            }

            var enrollmentByString = await _enrollmentService.GetByStringKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var enrollmentByNumeric = await _enrollmentService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var enrollmentByDateTime = await _enrollmentService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize, "Id");

            var res = enrollmentByString.Concat(enrollmentByString).DistinctBy(t => t.Id).Select(t => ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(t)).ToList();

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(string id)
        {
            var existingEnrollment = await _enrollmentService.GetByIdAsync(id);
            if (existingEnrollment == null)
            {
                return BadRequest(new {Message = "Enrollment not found"});
            }

            var res = ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(existingEnrollment);

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> AddEnrollment([FromBody] EnrollmentDTO enrollmentDTO)
        {
            if (enrollmentDTO == null)
            {
                return BadRequest(new { Message = "Invalid enrollment data." });
            }

            var enrollment = ModelMapUtils.MapBetweenClasses<EnrollmentDTO, Enrollment>(enrollmentDTO);

            var enrollments = await _enrollmentService.GetAllAsync(-1, -1, "Id");

            if (enrollments.Count() == 0)
            {
                enrollment.Id = "DK" + DateTime.UtcNow.ToString("ddMMyyyy") + "0000";
            }
            else
            {
                var last = enrollments.Last();

                int lastestIdNumber = int.Parse(last.Id.Substring(10));

                int newIdNumber = lastestIdNumber + 1;

                string newId = "DK" + DateTime.UtcNow.ToString("ddMMyyyy");

                for (int i = 0; i < 4 - newIdNumber.ToString().Length; i++) newId += "0";

                enrollment.Id = newId + newIdNumber.ToString();
            }

            await _enrollmentService.AddAsync(enrollment);

            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(enrollment));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(string id, [FromBody] EnrollmentDTO enrollmentDTO)
        {
            if (id == null || enrollmentDTO == null)
            {
                return BadRequest(new { Message = "Invalid enrollment data." });
            }

            var existingEnrollment = await _enrollmentService.GetByIdAsync(id);
            if (existingEnrollment == null)
            {
                return BadRequest(new { Message = "Enrollment not found." });
            }

            enrollmentDTO.Id = id;
            existingEnrollment.StudentId = enrollmentDTO.StudentId;
            existingEnrollment.CourseId = enrollmentDTO.CourseId;
            existingEnrollment.EnrollmentDate = enrollmentDTO.EnrollmentDate;
            existingEnrollment.Status = enrollmentDTO.Status;
            existingEnrollment.CreatedBy = enrollmentDTO.CreatedBy;

            await _enrollmentService.UpdateAsync(existingEnrollment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(string id)
        {
            var existingEnrollment = await _enrollmentService.GetByIdAsync(id);
            if (existingEnrollment == null)
            {
                return BadRequest(new { Message = "Enrollment not found." });
            }

            await _enrollmentService.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetEnrollmentByStudentId(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                return BadRequest(new { Message = "Student ID is required." });
            }

            var enrollments = await _enrollmentService.GetEnrollmentByStudentId(studentId);
            if (!enrollments.Any())
            {
                return NotFound(new { Message = "No enrollments found for the given student ID." });
            }

            var result = enrollments.Select(e => ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(e)).ToList();
            return Ok(result);
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetEnrollmentByCourseId(string courseId)
        {
            if (string.IsNullOrWhiteSpace(courseId))
            {
                return BadRequest(new { Message = "Course ID is required." });
            }

            var enrollments = await _enrollmentService.GetEnrollmentByCourseId(courseId);
            if (!enrollments.Any())
            {
                return NotFound(new { Message = "No enrollments found for the given course ID." });
            }

            var result = enrollments.Select(e => ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(e)).ToList();
            return Ok(result);
        }

        [HttpGet("student/{studentId}/course/{courseId}")]
        public async Task<IActionResult> GetEnrollmentByStudentIdCourseId(string studentId, string courseId)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(courseId))
            {
                return BadRequest(new { Message = "Student ID and Course ID are required." });
            }

            var enrollment = await _enrollmentService.GetEnrollmentByStudentIdCourseId(studentId, courseId);
            if (enrollment == null)
            {
                return NotFound(new { Message = "No enrollment found for the given student ID and course ID." });
            }

            var result = ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(enrollment);
            return Ok(result);
        }
    }
}
