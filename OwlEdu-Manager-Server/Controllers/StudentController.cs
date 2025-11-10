using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController :ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] Student student)
        {
            await _studentService.AddAsync(student);
            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }
            await _studentService.UpdateAsync(student);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            await _studentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
