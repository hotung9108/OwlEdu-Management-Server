using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;

namespace OwlEdu_Manager_Server.Controllers
{
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

            var enrollmentByString = _enrollmentService.GetByStringKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var enrollmentByNumeric = _enrollmentService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var enrollmentByDateTime = _enrollmentService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize, "Id");

            await Task.WhenAll(enrollmentByDateTime, enrollmentByNumeric, enrollmentByString);

            var res = enrollmentByString.Result.Concat(enrollmentByString.Result).DistinctBy(t => t.Id).Select(t => ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(t)).ToList();

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

            if (enrollments == null)
            {
                enrollment.Id = "DK" + DateTime.UtcNow.ToString("ddMMyyyy") + "0000";
            }
            else
            {
                var lastCourse = enrollments.Last();

                int lastestIdNumber = int.Parse(lastCourse.Id.Substring(2));

                int newIdNumber = lastestIdNumber + 1;

                string newId = "DK" + DateTime.UtcNow.ToString("ddMMyyyy");

                for (int i = 0; i < 4 - newIdNumber.ToString().Length; i++) newId += "0";

                enrollment.Id = newId + newIdNumber.ToString();
            }

            await _enrollmentService.AddAsync(enrollment);

            return CreatedAtAction(nameof(GetEnrollmentById), enrollment.Id, enrollmentDTO);
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
            var enrollment = ModelMapUtils.MapBetweenClasses<EnrollmentDTO, Enrollment>(enrollmentDTO);

            await _enrollmentService.UpdateAsync(enrollment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(string id)
        {
            var existingEnrollment = await _enrollmentService.GetByIdAsync(id);
            if (existingEnrollment == null)
            {
                return BadRequest(new {Message = "Enrollment not found."});
            }

            await _enrollmentService.DeleteAsync(existingEnrollment);
            return NoContent();
        }
    }
}
