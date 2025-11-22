    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;

namespace OwlEdu_Manager_Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        private readonly ScheduleService _scheduleService;
        private readonly ClassService _classService;
        private readonly CourseService _courseService;

        public ScheduleController(ScheduleService scheduleService, CourseService courseService, ClassService classService)
        {
            _scheduleService = scheduleService;
            _classService = classService;
            _courseService = courseService;
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

        [HttpDelete("Class/{classId}")]
        public async Task<IActionResult> DeleteSchedulesByClass(string classId)
        {
            // Lấy tất cả schedule của lớp
            var schedules = await _scheduleService.GetSchedulesByClassIdAsync(classId);

            if (!schedules.Any())
                return NotFound(new { Message = "No schedules found for this class." });

            // Xóa tất cả cùng lúc
            _scheduleService._dbSet.RemoveRange(schedules);
            await _scheduleService._context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Form")]
        public async Task<IActionResult> PostScheduleForm([FromBody] ScheduleFormDTO scheduleFormDTO)
        {
            if (scheduleFormDTO == null)
                return BadRequest("Invalid data");

            var _class = await _classService.GetByIdAsync(scheduleFormDTO.ClassId);
            if (_class == null) return BadRequest("Invalid data");

            var course = await _courseService.GetByIdAsync(_class.CourseId);
            int numberOfLessons = (int)course.NumberOfLessons;
            DateOnly startDate = (DateOnly)_class.StartDate;
            string teacherId = _class.TeacherId;

            // Xóa tất cả schedule hiện có của class
            var existingSchedules = await _scheduleService._dbSet
                .Where(s => s.ClassId == scheduleFormDTO.ClassId)
                .ToListAsync();
            if (existingSchedules.Any())
            {
                _scheduleService._dbSet.RemoveRange(existingSchedules);
                await _scheduleService._context.SaveChangesAsync();
            }

            // Map DaysOfWeek
            Dictionary<char, DayOfWeek> map = new()
    {
        { '0', DayOfWeek.Sunday }, { '1', DayOfWeek.Monday }, { '2', DayOfWeek.Tuesday },
        { '3', DayOfWeek.Wednesday }, { '4', DayOfWeek.Thursday }, { '5', DayOfWeek.Friday },
        { '6', DayOfWeek.Saturday }
    };
            var allowedDays = scheduleFormDTO.DaysOfWeek
                .Where(c => map.ContainsKey(c))
                .Select(c => map[c])
                .ToList();
            if (!allowedDays.Any())
                return BadRequest("DaysOfWeek is invalid");

            // Lấy maxIdNumber 1 lần trước vòng lặp
            var maxId = await _scheduleService.GetMaxScheduleIdAsync();
            int maxIdNumber = maxId != null ? int.Parse(maxId.Substring(2)) : 0;

            // Tạo schedule mới
            List<Schedule> schedules = new();
            DateTime current = startDate.ToDateTime(new TimeOnly(0, 0));
            int created = 0;

            while (created < numberOfLessons)
            {
                if (allowedDays.Contains(current.DayOfWeek))
                {
                    maxIdNumber++;
                    string newId = $"BH{maxIdNumber:D9}";

                    schedules.Add(new Schedule
                    {
                        Id = newId,
                        ClassId = scheduleFormDTO.ClassId,
                        Room = scheduleFormDTO.Room,
                        TeacherId = teacherId,
                        StartTime = scheduleFormDTO.StartTime,
                        EndTime = scheduleFormDTO.EndTime,
                        SessionDate = DateOnly.FromDateTime(current),
                    });

                    created++;
                }
                current = current.AddDays(1);
            }

            // Lưu tất cả schedule mới cùng lúc
            _scheduleService._dbSet.AddRange(schedules);
            await _scheduleService._context.SaveChangesAsync();

            return Ok(new { Message = "Tạo lịch học thành công", Count = schedules.Count });
        }

    }
}
