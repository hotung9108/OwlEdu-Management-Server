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
        private readonly PaymentService _paymentService;
        private readonly CourseService _courseService;
        private readonly StudentService _studentService;

        public EnrollmentController (EnrollmentService enrollmentService, PaymentService paymentService, CourseService courseService, StudentService studentService)
        {
            _enrollmentService = enrollmentService;
            _paymentService = paymentService;
            _courseService = courseService;
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments(string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (keyword.Trim() == "")
            {
                var enrollments = await _enrollmentService.GetAllAsync(pageNumber, pageSize, "Id");
                return Ok(enrollments.Select(t => ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(t)).ToList());
            }

            var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IEnumerable<Enrollment> finalResult = null;

            foreach (var key in keywords)
            {
                // Tìm theo từng loại
                var byStr = await _enrollmentService.GetByStringKeywordAsync(key, -1, pageSize, "Id");
                var byNum = await _enrollmentService.GetByNumericKeywordAsync(key, -1, pageSize, "Id");
                var byDate = await _enrollmentService.GetByDateTimeKeywordAsync(key, -1, pageSize, "Id");

                // Ghép kết quả của TỪ khóa hiện tại
                var unionForCurrentKeyword = byStr
                    .Concat(byNum)
                    .Concat(byDate)
                    .DistinctBy(t => t.Id);

                // Lần đầu → gán luôn
                if (finalResult == null)
                    finalResult = unionForCurrentKeyword;
                else
                    // Giao nhau để chỉ giữ các item match “mọi keyword”
                    finalResult = finalResult.Intersect(unionForCurrentKeyword);
            }

            // Map sang DTO
            var res = finalResult.Select(t => ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(t)).ToList();
            if (pageNumber != -1)
            {
                res = finalResult.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(t => ModelMapUtils.MapBetweenClasses<Enrollment, EnrollmentDTO>(t)).ToList();
            }

            if (res == null) res = new List<EnrollmentDTO>();

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
            enrollment.Status = "active";

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

            var course = await _courseService.GetByIdAsync(enrollment.CourseId);

            var student = await _studentService.GetByIdAsync(enrollment.StudentId);

            var payment = new Payment
            {
                Id = "",
                EnrollmentId = enrollment.Id,
                Amount = course.Fee,
                PaymentDate = null,
                FeeCollectorId = null,
                PayerId = student.AccountId,
                Method = null,
                Status = "pending"
            };

            var allPayments = await _paymentService.GetAllAsync(-1, -1, "Id");

            if (allPayments.Count() == 0)
            {
                payment.Id = "HD" + DateTime.UtcNow.ToString("ddMMyyyy") + "0000";
            }
            else
            {
                var last = allPayments.Last();

                var lastIdNumber = int.Parse(last.Id.Substring(10));

                var newIdNumber = lastIdNumber + 1;

                string newId = "HD" + DateTime.UtcNow.ToString("ddMMyyyy");

                for (int i = 0; i < 4 - newIdNumber.ToString().Length; i++) newId += "0";

                payment.Id = newId + newIdNumber.ToString();
            }

            await _paymentService.AddAsync(payment);

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

            existingEnrollment.StudentId = enrollmentDTO.StudentId;
            existingEnrollment.CourseId = enrollmentDTO.CourseId;
            existingEnrollment.EnrollmentDate = enrollmentDTO.EnrollmentDate;
            existingEnrollment.Status = enrollmentDTO.Status;
            existingEnrollment.CreatedBy = enrollmentDTO.CreatedBy;

            if (existingEnrollment.Status == "cancelled")
            {
                var existingPayment = await _paymentService.GetPayementByEnrollmentId(id);

                if (existingPayment != null)
                {
                    existingPayment.Status = "failed";

                    await _paymentService.UpdateAsync(existingPayment);
                }
            }

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
