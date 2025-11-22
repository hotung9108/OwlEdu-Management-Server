using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StaticController : Controller
    {
        private readonly StaticService _staticService;

        public StaticController(StaticService staticService)
        {
            _staticService = staticService;
        }

        [HttpGet("total-students")]
        public async Task<IActionResult> GetTotalStudents()
        {
            var count = await _staticService.GetTotalStudentCountAsync();
            return Ok(count);
        }

        [HttpGet("total-teachers")]
        public async Task<IActionResult> GetTotalTeachers()
        {
            var count = await _staticService.GetTotalTeacherCountAsync();
            return Ok(count);
        }

        [HttpGet("total-classes")]
        public async Task<IActionResult> GetTotalClasses()
        {
            var count = await _staticService.GetTotalClassCountAsync();
            return Ok(count);
        }

        [HttpGet("total-courses")]
        public async Task<IActionResult> GetTotalCourses()
        {
            var count = await _staticService.GetTotalCourseCountAsync();
            return Ok(count);
        }

        [HttpGet("revenue/by-date")]
        public async Task<IActionResult> GetTotalRevenueByDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            var revenue = await _staticService.GetTotalRevenueByDateAsync(date.Value);
            return Ok(revenue);
        }

        [HttpGet("revenue/by-month")]
        public async Task<IActionResult> GetTotalRevenueByMonth([FromQuery] int? year, [FromQuery] int? month)
        {
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            var revenue = await _staticService.GetTotalRevenueByMonthAsync(year.Value, month.Value);
            return Ok(revenue);
        }

        [HttpGet("revenue/by-year")]
        public async Task<IActionResult> GetTotalRevenueByYear([FromQuery] int? year)
        {
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            var revenue = await _staticService.GetTotalRevenueByYearAsync(year.Value);
            return Ok(revenue);
        }

        [HttpGet("revenue/paid/by-date")]
        public async Task<IActionResult> GetPaidRevenueByDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            var revenue = await _staticService.GetPaidRevenueByDateAsync(date.Value);
            return Ok(revenue);
        }

        [HttpGet("revenue/pending/by-date")]
        public async Task<IActionResult> GetPendingRevenueByDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            var revenue = await _staticService.GetPendingRevenueByDateAsync(date.Value);
            return Ok(revenue);
        }
        [HttpGet("revenue/paid/{year}")]
        public async Task<IActionResult> GetPaidRevenueByYear(int year)
        {
            var revenue = await _staticService.GetPaidRevenueByYearAsync(year);
            return Ok(new { Year = year, PaidRevenue = revenue });
        }

        [HttpGet("revenue/pending/{year}")]
        public async Task<IActionResult> GetPendingRevenueByYear(int year)
        {
            var revenue = await _staticService.GetPendingRevenueByYearAsync(year);
            return Ok(new { Year = year, PendingRevenue = revenue });
        }
        [HttpGet("enrollments/count/by-date")]
        public async Task<IActionResult> GetEnrollmentCountByDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            return Ok(await _staticService.GetEnrollmentCountByDateAsync(date.Value));
        }

        [HttpGet("enrollments/count/by-month")]
        public async Task<IActionResult> GetEnrollmentCountByMonth([FromQuery] int? year, [FromQuery] int? month)
        {
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            return Ok(await _staticService.GetEnrollmentCountByMonthAsync(year.Value, month.Value));
        }

        [HttpGet("enrollments/count/by-year")]
        public async Task<IActionResult> GetEnrollmentCountByYear([FromQuery] int? year)
        {
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            return Ok(await _staticService.GetEnrollmentCountByYearAsync(year.Value));
        }

        [HttpGet("enrollments/by-course/by-date")]
        public async Task<IActionResult> GetEnrollmentByCourseAndDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            return Ok(await _staticService.GetEnrollmentByCourseAndDateAsync(date.Value));
        }

        [HttpGet("enrollments/by-course/by-month")]
        public async Task<IActionResult> GetEnrollmentByCourseAndMonth([FromQuery] int? year, [FromQuery] int? month)
        {
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            return Ok(await _staticService.GetEnrollmentByCourseAndMonthAsync(year.Value, month.Value));
        }

        [HttpGet("enrollments/by-course/by-year")]
        public async Task<IActionResult> GetEnrollmentByCourseAndYear([FromQuery] int? year)
        {
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            return Ok(await _staticService.GetEnrollmentByCourseAndYearAsync(year.Value));
        }
        [HttpGet("attendance/status/by-date")]
        public async Task<IActionResult> GetAttendanceStatusByDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            return Ok(await _staticService.GetAttendanceStatusByDateAsync(date.Value));
        }

        [HttpGet("attendance/status/by-month")]
        public async Task<IActionResult> GetAttendanceStatusByMonth([FromQuery] int? year, [FromQuery] int? month)
        {
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            return Ok(await _staticService.GetAttendanceStatusByMonthAsync(year.Value, month.Value));
        }

        [HttpGet("attendance/status/by-year")]
        public async Task<IActionResult> GetAttendanceStatusByYear([FromQuery] int? year)
        {
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            return Ok(await _staticService.GetAttendanceStatusByYearAsync(year.Value));
        }
        [HttpGet("payments/details/by-date")]
        public async Task<IActionResult> GetPaymentDetailsByDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            return Ok(await _staticService.GetPaymentDetailsByDateAsync(date.Value));
        }

        [HttpGet("payments/details/by-month")]
        public async Task<IActionResult> GetPaymentDetailsByMonth([FromQuery] int? year, [FromQuery] int? month)
        {
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            return Ok(await _staticService.GetPaymentDetailsByMonthAsync(year.Value, month.Value));
        }

        [HttpGet("payments/details/by-year")]
        public async Task<IActionResult> GetPaymentDetailsByYear([FromQuery] int? year)
        {
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            return Ok(await _staticService.GetPaymentDetailsByYearAsync(year.Value));
        }
        [HttpGet("enrollments/details/by-date")]
        public async Task<IActionResult> GetEnrollmentDetailsByDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            return Ok(await _staticService.GetEnrollmentDetailsByDateAsync(date.Value));
        }

        [HttpGet("enrollments/details/by-month")]
        public async Task<IActionResult> GetEnrollmentDetailsByMonth([FromQuery] int? year, [FromQuery] int? month)
        {
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            return Ok(await _staticService.GetEnrollmentDetailsByMonthAsync(year.Value, month.Value));
        }

        [HttpGet("enrollments/details/by-year")]
        public async Task<IActionResult> GetEnrollmentDetailsByYear([FromQuery] int? year)
        {
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            return Ok(await _staticService.GetEnrollmentDetailsByYearAsync(year.Value));
        }
        [HttpGet("attendance/details/by-date")]
        public async Task<IActionResult> GetAttendanceDetailsByDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            return Ok(await _staticService.GetAttendanceDetailsByDateAsync(date.Value));
        }

        [HttpGet("attendance/details/by-month")]
        public async Task<IActionResult> GetAttendanceDetailsByMonth([FromQuery] int? year, [FromQuery] int? month)
        {
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            return Ok(await _staticService.GetAttendanceDetailsByMonthAsync(year.Value, month.Value));
        }

        [HttpGet("attendance/details/by-year")]
        public async Task<IActionResult> GetAttendanceDetailsByYear([FromQuery] int? year)
        {
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            return Ok(await _staticService.GetAttendanceDetailsByYearAsync(year.Value));
        }
        //[HttpGet("attendance/status")]
        //public async Task<IActionResult> GetAttendanceStatusByDate([FromQuery] DateTime? date)
        //{
        //    if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
        //    var list = await _staticService.GetAttendanceStatusByDateAsync(date.Value);
        //    return Ok(list);
        //}
        [HttpGet("scores/{classId}/by-date")]
        public async Task<IActionResult> GetScoreDetailsByDate(string classId, [FromQuery] DateTime? date)
        {
            if (string.IsNullOrEmpty(classId)) return BadRequest(new { Message = "Route parameter 'classId' is required." });
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            return Ok(await _staticService.GetScoreDetailsByDateAsync(classId, date.Value));
        }

        [HttpGet("scores/{classId}/by-month")]
        public async Task<IActionResult> GetScoreDetailsByMonth(string classId, [FromQuery] int? year, [FromQuery] int? month)
        {
            if (string.IsNullOrEmpty(classId)) return BadRequest(new { Message = "Route parameter 'classId' is required." });
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            return Ok(await _staticService.GetScoreDetailsByMonthAsync(classId, year.Value, month.Value));
        }

        [HttpGet("scores/{classId}/by-year")]
        public async Task<IActionResult> GetScoreDetailsByYear(string classId, [FromQuery] int? year)
        {
            if (string.IsNullOrEmpty(classId)) return BadRequest(new { Message = "Route parameter 'classId' is required." });
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            return Ok(await _staticService.GetScoreDetailsByYearAsync(classId, year.Value));
        }
        [HttpGet("scores/average/by-date")]
        public async Task<IActionResult> GetAverageScoreByClassAndDate([FromQuery] DateTime? date)
        {
            if (!date.HasValue) return BadRequest(new { Message = "Query parameter 'date' is required." });
            return Ok(await _staticService.GetAverageScoreByClassAndDateAsync(date.Value));
        }

        [HttpGet("scores/average/by-month")]
        public async Task<IActionResult> GetAverageScoreByClassAndMonth([FromQuery] int? year, [FromQuery] int? month)
        {
            if (!year.HasValue || !month.HasValue) return BadRequest(new { Message = "Query parameters 'year' and 'month' are required." });
            return Ok(await _staticService.GetAverageScoreByClassAndMonthAsync(year.Value, month.Value));
        }

        [HttpGet("scores/average/by-year")]
        public async Task<IActionResult> GetAverageScoreByClassAndYear([FromQuery] int? year)
        {
            if (!year.HasValue) return BadRequest(new { Message = "Query parameter 'year' is required." });
            return Ok(await _staticService.GetAverageScoreByClassAndYearAsync(year.Value));
        }
    }
}
