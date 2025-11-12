using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController :Controller
    {
        private readonly AccountService _accountService;
        private readonly StudentService _studentService;
        private readonly TeacherService _teacherService;
        public AccountController(AccountService accountService, StudentService studentService, TeacherService teacherService)
        {
            _accountService = accountService;
            _teacherService = teacherService;
            _studentService = studentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var accounts = await _accountService.GetAllAsync(pageNumber, pageSize);
            var accountResponses = accounts.Select(account => new AccountResponse
            {
                Id = account.Id,
                Username = account.Username,
                Avatar = account.Avatar,
                Role = account.Role,
                Status = account.Status,
                Email = account.Email,
                CreatedAt = account.CreatedAt,
                UpdateAt = account.UpdateAt
            });
            return Ok(accountResponses);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound(new { Message = "Account not found." });
            }
            var accountResponse = new AccountResponse
            {
                Id = account.Id,
                Username = account.Username,
                Avatar = account.Avatar,
                Role = account.Role,
                Status = account.Status,
                Email = account.Email,
                CreatedAt = account.CreatedAt,
                UpdateAt = account.UpdateAt
            };

            return Ok(accountResponse);
        }
        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] AccountRequest accountRequest)
        {
            if (accountRequest == null)
            {
                return BadRequest(new { Message = "Account data is required." });
            }
            string baseUsername;
            if (accountRequest.Role?.ToLower() == "student" &&  accountRequest.StudentRequest != null)
            {
                baseUsername = string.Join("", accountRequest.StudentRequest.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0])).ToLower();
            }
            else if(accountRequest.Role?.ToLower() == "teacher"  && accountRequest.TeacherRequest != null){
                baseUsername = string.Join("", accountRequest.TeacherRequest.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0])).ToLower();
            }
            else
            {
                baseUsername = accountRequest.Username;
            }
            string username = baseUsername;
            int suffix = 1;

            while (await _accountService.FindAsync(a => a.Username == username, 1, 1) is { } accounts && accounts.Any())
            {
                username = $"{baseUsername}{suffix}";
                suffix++;
            }
            string newAccountId = "U" + new string('0', 9);
            var existingAccounts = await _accountService.FindAsync(
                a => a.Id.StartsWith("U"), 1, int.MaxValue);
            int maxAccountSequence = existingAccounts.Select(a => int.TryParse(a.Id.Substring(1), out var num) ? num : 0).DefaultIfEmpty(0).Max();
            newAccountId = $"U{(maxAccountSequence + 1):D9}";
            //string username;
            //if (accountRequest.Role?.ToLower() == "student" && accountRequest.StudentRequest != null)
            //{
            //    username = string.Join("", accountRequest.StudentRequest.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0])).ToLower();

            //}
            //else if(accountRequest.Role?.ToLower() == "teacher" && accountRequest.TeacherRequest != null)
            //{
            //    username = string.Join("", accountRequest.TeacherRequest.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word[0])).ToLower();
            //}
            //else
            //{
            //    username = accountRequest.Username;
            //}
            var newAccount = new Account
            {
                Id = newAccountId,
                Username = username,
                Password = accountRequest.Password,
                Avatar = accountRequest.Avatar,
                Role = accountRequest.Role,
                Status = accountRequest.Status,
                Email = accountRequest.Email,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            
            if (accountRequest.Role?.ToLower() == "student" && accountRequest.StudentRequest != null)
            {
                string currentDate = DateTime.UtcNow.ToString("ddMMyyyy");
                var existingStudents = await _studentService.FindAsync(
                    s => s.Id.StartsWith($"HV{currentDate}"), 1, int.MaxValue);
                int maxStudentSequence = existingStudents.Select(s => int.TryParse(s.Id.Substring(10), out var num) ? num : 0).DefaultIfEmpty(0).Max();
                string newStudentId = $"HV{currentDate}{(maxStudentSequence + 1):D3}";

                var newStudent = new Student
                {
                    Id = newStudentId,
                    AccountId = newAccount.Id,
                    FullName = accountRequest.StudentRequest.FullName,
                    BirthDate = accountRequest.StudentRequest.BirthDate,
                    PhoneNumber = accountRequest.StudentRequest.PhoneNumber,
                    Address = accountRequest.StudentRequest.Address,
                    Gender = accountRequest.StudentRequest.Gender
                };
                //newAccount.Students.Add(newStudent);
                await _accountService.AddAsync(newAccount);
                await _studentService.AddAsync(newStudent);
            }
            else if (accountRequest.Role?.ToLower() == "teacher" && accountRequest.TeacherRequest != null)
            {
                string currentDate = DateTime.UtcNow.ToString("ddMMyyyy");
                var existingTeachers = await _teacherService.FindAsync(s => s.Id.StartsWith($"GV{currentDate}"), 1, int.MaxValue);
                int maxTeacherSequence = existingTeachers.Select(s => int.TryParse(s.Id.Substring(10), out var num) ? num : 0).DefaultIfEmpty(0).Max();
                string newTeacherId = $"GV{currentDate}{(maxTeacherSequence + 1):D3}";

                var newTeacher = new Teacher
                {
                    Id = newTeacherId,
                    AccountId = newAccount.Id,
                    FullName = accountRequest.TeacherRequest.FullName,
                    Specialization = accountRequest.TeacherRequest.Specialization,
                    Qualification = accountRequest.TeacherRequest.Qualification,
                    PhoneNumber = accountRequest.TeacherRequest.PhoneNumber,
                    Address = accountRequest.TeacherRequest.Address,
                    Gender = accountRequest.TeacherRequest.Gender
                };
                //newAccount.Teachers.Add(newTeacher);
                await _accountService.AddAsync(newAccount);
                await _teacherService.AddAsync(newTeacher);
            }
            else
            {
                await _accountService.AddAsync(newAccount);

            }
            var accountResponse = new AccountResponse
            {
                Id = newAccount.Id,
                Username = newAccount.Username,
                Avatar = newAccount.Avatar,
                Role = newAccount.Role,
                Status = newAccount.Status,
                Email = newAccount.Email,
                CreatedAt = newAccount.CreatedAt,
                UpdateAt = newAccount.UpdateAt
            };
            return CreatedAtAction(nameof(GetAccountById), new { id = newAccount.Id }, accountResponse);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] AccountRequest accountRequest)
        {
            if (accountRequest == null)
            {
                return BadRequest(new { Message = "Invalid account data." });
            }
            var existingAccount = await _accountService.GetByIdAsync(id);
            if (existingAccount == null)
            {
                return NotFound(new { Message = "Account not found." });
            }
            existingAccount.Id = id;
            existingAccount.Username = accountRequest.Username;
            existingAccount.Password = accountRequest.Password;
            existingAccount.Avatar = accountRequest.Avatar;
            existingAccount.Role = accountRequest.Role;
            existingAccount.Status = accountRequest.Status;
            existingAccount.Email = accountRequest.Email;
            existingAccount.UpdateAt = DateTime.UtcNow;
            await _accountService.UpdateAsync(existingAccount);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var existingAccount = await _accountService.GetByIdAsync(id);
            if (existingAccount == null)
            {
                return NotFound(new { Message = "Account not found." });
            }
            await _accountService.DeleteAsync(id);
            return NoContent();
        }
        //[HttpGet("search/string")]
        //public async Task<IActionResult> SearchAccountsByString([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        return BadRequest(new { Message = "Keyword cannot be empty." });
        //    }

        //    var accounts = await _accountService.GetByStringKeywordAsync(keyword, pageNumber, pageSize);
        //    return Ok(accounts);
        //}
        //[HttpGet("search/number")]
        //public async Task<IActionResult> SearchAccountsByNumber([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        return BadRequest(new { Message = "Keyword cannot be empty." });
        //    }

        //    var accounts = await _accountService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize);
        //    return Ok(accounts);
        //}

        //[HttpGet("search/datetime")]
        //public async Task<IActionResult> SearchAccountsByDateTime([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        return BadRequest(new { Message = "Keyword cannot be empty." });
        //    }

        //    var accounts = await _accountService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize);
        //    return Ok(accounts);
        //}
    }
}
