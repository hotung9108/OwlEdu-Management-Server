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

            var attendancesByString = _attendanceService.GetByStringKeywordAsync(keyword, pageNumber, pageSize, "ScheduleId", "StudentId");
            var attendancesByNumeric = _attendanceService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize, "ScheduleId", "StudentId");

            await Task.WhenAll(attendancesByString, attendancesByNumeric);

            var res = attendancesByString.Result.Concat(attendancesByNumeric.Result).DistinctBy(t => new {t.ScheduleId, t.StudentId}).Select(t => ModelMapUtils.MapBetweenClasses<Attendance, AttendanceDTO>(t)).ToList(); 

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
            return CreatedAtAction(nameof(GetAttendanceById), new {attendance.ScheduleId, attendance.StudentId}, attendanceDTO);
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
            var attendance = ModelMapUtils.MapBetweenClasses<AttendanceDTO, Attendance>(attendanceDTO);

            await _attendanceService.UpdateAsync(attendance);
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

            await _attendanceService.DeleteAsync(existingAttendance);
            return NoContent();
        }
    }
}
