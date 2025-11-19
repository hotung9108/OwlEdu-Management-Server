using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OwlEdu_Manager_Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController :ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly AccountService _accountService;
        public StudentController(StudentService studentService, AccountService accountService)
        {
            _studentService = studentService;
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllStudents([FromQuery] string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            //var students = await _studentService.GetAllAsync(pageNumber, pageSize, "Id");
            //var studentResponses = students.Select(student => new StudentResponse
            //{
            //    Id = student.Id,
            //    FullName = student.FullName,
            //    PhoneNumber = student.PhoneNumber,
            //    Address = student.Address,
            //    Gender = student.Gender
            //});
            //return Ok(studentResponses);

            if (string.IsNullOrWhiteSpace(keyword))
            {
                var students = await _studentService.GetAllAsync(pageNumber, pageSize, "Id");
                return Ok(students.Select(ModelMapUtils.MapBetweenClasses<Student, StudentResponse>).ToList());
            }

            var studentByString = await _studentService.GetByStringKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var studentByNumeric = await _studentService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var studentByDateTime = await _studentService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize, "Id");

            var res = studentByString
                .Concat(studentByNumeric)
                .Concat(studentByDateTime)
                .DistinctBy(t => t.Id)
                .Select(ModelMapUtils.MapBetweenClasses<Student, StudentResponse>)
                .ToList();

            return Ok(res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound(new { Message = "Not found student" });
            }

            if (student.AccountId == null)
            {
                return NotFound(new { Message = "Account not found." });
            }
            var account = await _accountService.GetByIdAsync(student.AccountId);
            var studentDetailResponse = new StudentDetailResponse
            {
                Id = student.Id,
                FullName = student.FullName,
                BirthDate = student.BirthDate,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                Gender = student.Gender,
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
            return Ok(studentDetailResponse);
        }
        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] StudentRequest studentDTO)
        {
            if (studentDTO == null)
            {
                return BadRequest(new { Message = "Student data is required." });
            }
            string baseUsername = string.Join("", studentDTO.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0])).ToLower();
            string username = baseUsername;
            int suffix = 1;

            while (await _accountService.FindAsync(a => a.Username == username, 1, 1) is { } accounts && accounts.Any())
            {
                username = $"{baseUsername}{suffix}";
                suffix++;
            }
            string newAccountId = "U" + new string('0', 9);
            var existingAccounts = await _accountService.FindAsync(a => a.Id.StartsWith("U"), 1, int.MaxValue);
            int maxAccountSequence = existingAccounts.Select(a => int.TryParse(a.Id.Substring(1), out var num) ? num : 0).DefaultIfEmpty(0).Max();
            newAccountId = $"U{(maxAccountSequence + 1):D9}";
            //string username = string.Join("", studentDTO.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0])).ToLower();
            string currentDate = DateTime.UtcNow.ToString("ddMMyyyy");
            var existingStudents = await _studentService.FindAsync(
                s => s.Id.StartsWith($"HV{currentDate}"), 1, int.MaxValue);
            int maxStudentSequence = existingStudents
                .Select(s => int.TryParse(s.Id.Substring(10), out var num) ? num : 0)
                .DefaultIfEmpty(0)
                .Max();
            string newStudentId = $"HV{currentDate}{(maxStudentSequence + 1):D3}";

            var newAccount = new Account
            {
                Id = newAccountId,
                Username = username,
                Password = "OwlEdu",
                Role = "student",
                Status = true,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
            };
            var newStudent = new Student
            {
                Id = newStudentId,
                AccountId = newAccountId,
                FullName = studentDTO.FullName,
                BirthDate = studentDTO.BirthDate,
                PhoneNumber = studentDTO.PhoneNumber,
                Address = studentDTO.Address,
                Gender = studentDTO.Gender
            };
            newAccount.Students.Add(newStudent);
            await _studentService.AddAsync(newStudent);
            await _accountService.AddAsync(newAccount);
            var response = new StudentDetailResponse
            {
                Id = newStudent.Id,
                FullName = newStudent.FullName,
                PhoneNumber = newStudent.PhoneNumber,
                Address = newStudent.Address,
                Gender = newStudent.Gender,
                BirthDate = newStudent.BirthDate,
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
                //AccountId = newAccount.Id,
                //Username = newAccount.Username,
                //Avatar = newAccount.Avatar,
                //Role = newAccount.Role,
                //Status = newAccount.Status,
                //Email = newAccount.Email,
                //CreatedAt = newAccount.CreatedAt,
                //UpdateAt = newAccount.UpdateAt
            };
            return CreatedAtAction(nameof(GetStudentById), new { id = newStudent.Id }, response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            await _studentService.DeleteAsync(id);
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] StudentRequest studentDTO)
        {
            if (studentDTO == null)
            {
                return BadRequest(new { Message = "Invalid account data or mismatched ID." });
            }
            var existingStudent = await _studentService.GetByIdAsync(id);
            if (existingStudent == null)
            {
                return NotFound(new { Message = "Student not found." });
            }
            // Ensure the primary key (Id) is set
            existingStudent.Id = id;
            existingStudent.FullName = studentDTO.FullName;
            existingStudent.BirthDate = studentDTO.BirthDate;
            existingStudent.PhoneNumber = studentDTO.PhoneNumber;
            existingStudent.Address = studentDTO.Address;
            existingStudent.Gender = studentDTO.Gender;
            await _studentService.UpdateAsync(existingStudent);
            return NoContent();
        }
        //[HttpGet("by-account/{accountId}")]
        //public async Task<IActionResult> GetStudentByAccountId(string accountId)
        //{
        //    var student = await _studentService.GetStudentByAccountIdAsync(accountId);
        //    var account = await _accountService.GetByIdAsync(accountId);
        //    if (student == null)
        //    {
        //        return NotFound(new { Message = "Student not found for the given account ID." });
        //    }

        //    var studentResponse = new StudentDetailResponse
        //    {
        //        Id = student.Id,
        //        FullName = student.FullName,
        //        BirthDate = student.BirthDate,
        //        PhoneNumber = student.PhoneNumber,
        //        Address = student.Address,
        //        Gender = student.Gender,
        //        accountResponse = student.Account != null ? new AccountResponse
        //        {
        //            Id = accountId,
        //            Username = account.Username,
        //            Avatar = account.Avatar,
        //            Role = account.Role,
        //            Status = account.Status,
        //            Email = account.Email,
        //            CreatedAt = account.CreatedAt,
        //            UpdateAt = account.UpdateAt
        //        } : null
        //    };

        //    return Ok(studentResponse);
        //}
        //public StudentController(StudentService studentService)
        //{
        //    _studentService = studentService;
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetAllStudents()
        //{
        //    var students = await _studentService.GetAllAsync();
        //    return Ok(students);
        //}
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetStudentById(string id)
        //{
        //    var student = await _studentService.GetByIdAsync(id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(student);
        //}
        //[HttpPost]
        //public async Task<IActionResult> AddStudent([FromBody] Student student)
        //{
        //    await _studentService.AddAsync(student);
        //    return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        //}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateStudent(string id, [FromBody] Student student)
        //{
        //    if (id != student.Id)
        //    {
        //        return BadRequest();
        //    }
        //    await _studentService.UpdateAsync(student);
        //    return NoContent();
        //}
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStudent(string id)
        //{
        //    await _studentService.DeleteAsync(id);
        //    return NoContent();
        //}
    }
}
