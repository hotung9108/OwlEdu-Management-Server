using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSchedules(int pageNumber = 1, int pageSize = 10)
        {
            var schedules = await _scheduleService.GetAllAsync(pageNumber, pageSize, "Id");
            var response = schedules.Select(schedule => new ScheduleResponse
            {
                Id = schedule.Id,
                ClassId = schedule.ClassId,
                ClassName = schedule.Class?.Name,
                SessionDate = schedule.SessionDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                TeacherId = schedule.TeacherId,
                TeacherName = schedule.Teacher?.FullName
            });
            return Ok(response);
        }

        [HttpGet("Class/{classId}")]
        public async Task<IActionResult> GetSchedulesByClass(string classId)
        {
            var schedules = await _scheduleService.GetSchedulesByClassIdAsync(classId);
            var response = schedules.Select(schedule => new ScheduleResponse
            {
                Id = schedule.Id,
                ClassId = schedule.ClassId,
                ClassName = schedule.Class?.Name,
                SessionDate = schedule.SessionDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                TeacherId = schedule.TeacherId,
                TeacherName = schedule.Teacher?.FullName
            });
            return Ok(response);
        }

        [HttpGet("Teacher/{teacherId}")]
        public async Task<IActionResult> GetSchedulesByTeacher(string teacherId)
        {
            var schedules = await _scheduleService.GetSchedulesByTeacherIdAsync(teacherId);
            var response = schedules.Select(schedule => new ScheduleResponse
            {
                Id = schedule.Id,
                ClassId = schedule.ClassId,
                ClassName = schedule.Class?.Name,
                SessionDate = schedule.SessionDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                TeacherId = schedule.TeacherId,
                TeacherName = schedule.Teacher?.FullName
            });
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddSchedule([FromBody] ScheduleRequest scheduleRequest)
        {
            if (scheduleRequest == null)
            {
                return BadRequest(new { Message = "Invalid schedule data." });
            }
            var maxId = await _scheduleService.GetMaxScheduleIdAsync();
            var newIdNumber = maxId != null ? int.Parse(maxId.Substring(2)) + 1 : 1;
            var newId = $"BH{newIdNumber:D9}";
            var schedule = new Schedule
            {
                Id = newId,
                ClassId = scheduleRequest.ClassId,
                SessionDate = scheduleRequest.SessionDate,
                StartTime = scheduleRequest.StartTime,
                EndTime = scheduleRequest.EndTime,
                Room = scheduleRequest.Room,
                TeacherId = scheduleRequest.TeacherId
            };

            await _scheduleService.AddAsync(schedule);

            var response = new ScheduleResponse
            {
                Id = schedule.Id,
                ClassId = schedule.ClassId,
                SessionDate = schedule.SessionDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room,
                TeacherId = schedule.TeacherId
            };

            return CreatedAtAction(nameof(GetSchedulesByClass), new { classId = schedule.ClassId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(string id, [FromBody] ScheduleRequest scheduleRequest)
        {
            if (scheduleRequest == null)
            {
                return BadRequest(new { Message = "Invalid schedule data." });
            }

            var existingSchedule = await _scheduleService.GetByIdAsync(id);
            if (existingSchedule == null)
            {
                return NotFound(new { Message = "Schedule not found." });
            }

            existingSchedule.ClassId = scheduleRequest.ClassId;
            existingSchedule.SessionDate = scheduleRequest.SessionDate;
            existingSchedule.StartTime = scheduleRequest.StartTime;
            existingSchedule.EndTime = scheduleRequest.EndTime;
            existingSchedule.Room = scheduleRequest.Room;
            existingSchedule.TeacherId = scheduleRequest.TeacherId;

            await _scheduleService.UpdateAsync(existingSchedule);

            var response = new ScheduleResponse
            {
                Id = existingSchedule.Id,
                ClassId = existingSchedule.ClassId,
                SessionDate = existingSchedule.SessionDate,
                StartTime = existingSchedule.StartTime,
                EndTime = existingSchedule.EndTime,
                Room = existingSchedule.Room,
                TeacherId = existingSchedule.TeacherId
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(string id)
        {
            var existingSchedule = await _scheduleService.GetByIdAsync(id);
            if (existingSchedule == null)
            {
                return NotFound(new { Message = "Schedule not found." });
            }

            await _scheduleService.DeleteAsync(id);

            return NoContent();
        }
    }
}
