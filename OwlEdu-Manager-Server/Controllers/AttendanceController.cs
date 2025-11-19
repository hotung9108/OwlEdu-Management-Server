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
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAttendances([FromQuery] string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (keyword == "")
            { 
                var attendances = await _attendanceService.GetAllAsync(pageNumber, pageSize, "ScheduleId", "StudentId");
                return Ok(attendances.Select(t => ModelMapUtils.MapBetweenClasses<Attendance, AttendanceDTO>(t)).ToList());
            }

            var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IEnumerable<Attendance> finalResult = null;

            foreach (var key in keywords)
            {
                // Tìm theo từng loại
                var byStr = await _attendanceService.GetByStringKeywordAsync(key, -1, pageSize, "ScheduleId", "StudentId");
                var byNum = await _attendanceService.GetByNumericKeywordAsync(key, -1, pageSize, "ScheduleId", "StudentId");

                // Ghép kết quả của TỪ khóa hiện tại
                var unionForCurrentKeyword = byStr
                    .Concat(byNum)
                    .DistinctBy(t => new { t.ScheduleId, t.StudentId });

                // Lần đầu → gán luôn
                if (finalResult == null)
                    finalResult = unionForCurrentKeyword;
                else
                    // Giao nhau để chỉ giữ các item match “mọi keyword”
                    finalResult = finalResult.Intersect(unionForCurrentKeyword);
            }

            // Map sang DTO
            var res = finalResult.Select(t => ModelMapUtils.MapBetweenClasses<Attendance, AttendanceDTO>(t)).ToList();
            if (pageNumber != -1)
            {
                res = finalResult.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(t => ModelMapUtils.MapBetweenClasses<Attendance, AttendanceDTO>(t)).ToList();
            }

            if (res == null) res = new List<AttendanceDTO>();

            return Ok(res);
        }

        [HttpGet("{scheduleId}/{studentId}")]
        public async Task<IActionResult> GetAttendanceById(string scheduleId = "", string studentId = "")
        {
            var existingAttendance = await _attendanceService.GetAttendanceByScheduleIdStudentId(scheduleId, studentId);
            if (existingAttendance == null)
            {
                return BadRequest(new {Message = "Attendance not found."});
            }
            var res = ModelMapUtils.MapBetweenClasses<Attendance, AttendanceDTO>(existingAttendance);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> AddAttendance([FromBody] AttendanceDTO attendanceDTO)
        {
            if (attendanceDTO == null)
            {
                return BadRequest(new {Message = "Invalid attendance data."});
            }

            var attendance = ModelMapUtils.MapBetweenClasses<AttendanceDTO, Attendance>(attendanceDTO);

            await _attendanceService.AddAsync(attendance);
            return CreatedAtAction(nameof(GetAttendanceById), new {attendance.ScheduleId, attendance.StudentId}, ModelMapUtils.MapBetweenClasses<Attendance, AttendanceDTO>(attendance));
        }

        [HttpPut("{scheduleId}/{studentId}")]
        public async Task<IActionResult> UpdateAttendance(string scheduleId, string studentId, [FromBody] AttendanceDTO attendanceDTO)
        {
            if (scheduleId == null || studentId == null || attendanceDTO == null)
            {
                return BadRequest(new { Message = "Invalid attendance data." });
            }

            var existingAttendance = await _attendanceService.GetAttendanceByScheduleIdStudentId(scheduleId, studentId);
            if (existingAttendance == null)
            {
                return BadRequest(new {Message = "Attendance not found."});
            }

            attendanceDTO.ScheduleId = scheduleId;
            attendanceDTO.StudentId = studentId;
            existingAttendance.Status = attendanceDTO.Status;
            existingAttendance.Note = attendanceDTO.Note;
            existingAttendance.TeacherId = attendanceDTO.TeacherId;

            await _attendanceService.UpdateAsync(existingAttendance);
            return NoContent();
        }

        [HttpDelete("{scheduleId}/{studentId}")]
        public async Task<IActionResult> DeleteAttendance(string scheduleId, string studentId)
        {
            var existingAttendance = await _attendanceService.GetAttendanceByScheduleIdStudentId(scheduleId, studentId);
            if (existingAttendance == null)
            {
                return BadRequest(new { Message = "Attendance not found." });
            }

            await _attendanceService.DeleteAttendance(scheduleId, studentId);
            return NoContent();
        }
    }
}
