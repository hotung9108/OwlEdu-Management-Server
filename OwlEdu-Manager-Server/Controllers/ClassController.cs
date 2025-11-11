using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly ClassService _classService;

        public ClassController(ClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClasses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var classes = await _classService.GetAllAsync(pageNumber, pageSize);
            return Ok(classes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassById(string id)
        {
            var _class = await _classService.GetByIdAsync(id);
            if (_class == null)
            {
                return BadRequest(new { Message = "Class not found." });
            }
            return Ok(_class);
        }

        [HttpPost]
        public async Task<IActionResult> AddClass([FromBody] Class _class)
        {
            if (_class == null)
            {
                return BadRequest(new { Message = "Invalid class data."});
            }

            await _classService.AddAsync(_class);
            return CreatedAtAction(nameof(GetClassById), new {id = _class.Id}, _class);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(string id, [FromBody] Class _class)
        {
            if (id == null || _class == null)
            {
                return BadRequest(new { Message = "Invalid class data."});
            }

            var existingClass = await _classService.GetByIdAsync(id);
            if (existingClass == null)
            {
                return BadRequest(new {Message = "Class not found." });
            }

            await _classService.UpdateAsync(_class);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(string id)
        {
            var existingClass = await _classService.GetByIdAsync(id);
            if (existingClass == null)
            {
                return BadRequest(new { Message = "Class not found." });
            }

            await _classService.DeleteAsync(id);
            return NoContent();
        }
    }
}
