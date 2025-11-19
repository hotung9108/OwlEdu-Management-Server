using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class TeacherController : Controller
    {
        private readonly TeacherService _teacherService;
        private readonly AccountService _accountService;
        public TeacherController(TeacherService teacherService, AccountService accountService)
        {
            _teacherService = teacherService;
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTeachers([FromQuery] string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            //var teachers = await _teacherService.GetAllAsync(pageNumber, pageSize, "Id");
            //var teacherResponses = teachers.Select(teacher => new TeacherResponse
            //{
            //    Id = teacher.Id,
            //    FullName = teacher.FullName,
            //    Specialization = teacher.Specialization,
            //    Qualification = teacher.Qualification,
            //    PhoneNumber = teacher.PhoneNumber,
            //    Address = teacher.Address,
            //    Gender = teacher.Gender

            //});
            //return Ok(teacherResponses);
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var teachers = await _teacherService.GetAllAsync(pageNumber, pageSize, "Id");
                return Ok(teachers.Select(ModelMapUtils.MapBetweenClasses<Teacher, TeacherResponse>).ToList());
            }

            var teacherByString = await _teacherService.GetByStringKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var teacherByNumeric = await _teacherService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var teacherByDateTime = await _teacherService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize, "Id");

            var res = teacherByString
                .Concat(teacherByNumeric)
                .Concat(teacherByDateTime)
                .DistinctBy(t => t.Id)
                .Select(ModelMapUtils.MapBetweenClasses<Teacher, TeacherResponse>)
                .ToList();

            return Ok(res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacherById(string id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);

            if (teacher == null)
            {
                return NotFound(new { Message = "Teacher not found." });
            }
            if (teacher.AccountId == null)
            {
                return NotFound(new { Message = "Account not found." });
            }
            var account = await _accountService.GetByIdAsync(teacher.AccountId);
            var teacherDetailResponse = new TeacherDetailResponse
            {
                Id = teacher.Id,
                FullName = teacher.FullName,
                Specialization = teacher.Specialization,
                Qualification = teacher.Qualification,
                PhoneNumber = teacher.PhoneNumber,
                Address = teacher.Address,
                Gender = teacher.Gender,

                accountResponse = account != null ? new AccountResponse
                {
                    Id = account.Id,
                    Username = account.Username,
                    Avatar = account.Avatar,
                    Role = account.Role,
                    Status = account.Status,
                    Email = account.Email,
                    CreatedAt = account.CreatedAt,
                    UpdateAt = account.UpdateAt
                } : null
            };
            return Ok(teacherDetailResponse);
        }
        [HttpPost]
        public async Task<IActionResult> AddTeacher([FromBody] TeacherRequest teacherRequest)
        {
            if (teacherRequest == null)
            {
                return BadRequest(new { Message = "Teacher data is required." });
            }
            string baseUsername = string.Join("", teacherRequest.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0])).ToLower();
            string username = baseUsername;
            int suffix = 1;

            while (await _accountService.FindAsync(a => a.Username == username, 1, 1) is { } accounts && accounts.Any())
            {
                username = $"{baseUsername}{suffix}";
                suffix++;
            }
            var existingAccounts = await _accountService.FindAsync(a => a.Id.StartsWith("U"), 1, int.MaxValue);
            int maxAccountSequence = existingAccounts.Select(a => int.TryParse(a.Id.Substring(1), out var num) ? num : 0).DefaultIfEmpty(0).Max();
            string newAccountId = "U" + new string('0', 9);
            newAccountId = $"U{(maxAccountSequence + 1):D9}";

            //string username = string.Join("", teacherRequest.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0])).ToLower();
            string currentDate = DateTime.UtcNow.ToString("ddMMyyyy");
            var existingTeachers = await _teacherService.FindAsync(
                s => s.Id.StartsWith($"GV{currentDate}"), 1, int.MaxValue);
            int maxTeacherSequence = existingTeachers.Select(s => int.TryParse(s.Id.Substring(10), out var num) ? num : 0).DefaultIfEmpty(0).Max();
            string newTeacherId = $"GV{currentDate}{(maxTeacherSequence + 1):D3}";
            var newAccount = new Account
            {
                Id = newAccountId,
                Username = username,
                Password = "OwlEdu",
                Role = "student",
                Status = true,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            var newTeacher = new Teacher
            {
                AccountId = newAccountId,
                Id = newTeacherId,
                FullName = teacherRequest.FullName,
                Specialization = teacherRequest.Specialization,
                Qualification = teacherRequest.Qualification,
                PhoneNumber = teacherRequest.PhoneNumber,
                Address = teacherRequest.Address,
                Gender = teacherRequest.Gender
            };
            //newAccount.Teachers.Add(newTeacher);
            await _teacherService.AddAsync(newTeacher);
            await _accountService.AddAsync(newAccount);
            var teacherResponse = new TeacherDetailResponse
            {
                Id = newTeacherId,
                FullName = teacherRequest.FullName,
                Specialization = teacherRequest.Specialization,
                Qualification = teacherRequest.Qualification,
                PhoneNumber = teacherRequest.PhoneNumber,
                Address = teacherRequest.Address,
                Gender = teacherRequest.Gender,

                accountResponse = newAccount != null ? new AccountResponse
                {
                    Id = newAccount.Id,
                    Username = newAccount.Username,
                    Avatar = newAccount.Avatar,
                    Role = newAccount.Role,
                    Status = newAccount.Status,
                    Email = newAccount.Email,
                    CreatedAt = newAccount.CreatedAt,
                    UpdateAt = newAccount.UpdateAt
                } : null
            };
            return CreatedAtAction(nameof(GetTeacherById), new { id = newTeacher.Id }, teacherResponse);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(string id, [FromBody] TeacherRequest teacherRequest)
        {
            if (teacherRequest == null)
            {
                return BadRequest(new { Message = "Invalid teacher data." });
            }
            var existingTeacher = await _teacherService.GetByIdAsync(id);
            if (existingTeacher == null)
            {
                return NotFound(new { Message = "Teacher not found." });
            }
            existingTeacher.Id = id;
            existingTeacher.FullName = teacherRequest.FullName;
            existingTeacher.Specialization = teacherRequest.Specialization;
            existingTeacher.Qualification = teacherRequest.Qualification;
            existingTeacher.PhoneNumber = teacherRequest.PhoneNumber;
            existingTeacher.Address = teacherRequest.Address;
            existingTeacher.Gender = teacherRequest.Gender;
            await _teacherService.UpdateAsync(existingTeacher);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
            {
                return NotFound(new { Message = "Teacher not found." });
            }

            await _teacherService.DeleteAsync(id);
            return NoContent();
        }
    }
}
