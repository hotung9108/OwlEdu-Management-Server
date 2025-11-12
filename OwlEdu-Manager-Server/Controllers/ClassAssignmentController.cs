using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;

namespace OwlEdu_Manager_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassAssignmentController : ControllerBase
    {
        private readonly ClassAssignmentService _classAssignmentService;

        public ClassAssignmentController(ClassAssignmentService classAssignmentService)
        {
            _classAssignmentService = classAssignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClassAssignments([FromQuery] string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (keyword == "")
            {
                var classAssignment = await _classAssignmentService.GetAllAsync(pageNumber, pageSize, "ClassId", "StudentId");
                return Ok(classAssignment.Select(t => ModelMapUtils.MapBetweenClasses<ClassAssignment, ClassAssignmentDTO>(t)).ToList());
            }

            var classAssignmentByString = _classAssignmentService.GetByStringKeywordAsync(keyword, pageNumber, pageSize, "ClassId", "StudentId");
            var classAssignmentByDateTime = _classAssignmentService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize, "ClassId", "StudentId");

            await Task.WhenAll(classAssignmentByString, classAssignmentByDateTime);

            var res = classAssignmentByString.Result.Concat(classAssignmentByDateTime.Result).DistinctBy(t => new { t.StudentId, t.ClassId }).Select(t => ModelMapUtils.MapBetweenClasses<ClassAssignment, ClassAssignmentDTO>(t)).ToList();

            return Ok(res);
        }

        [HttpGet("{classId}/{studentId}")]
        public async Task<IActionResult> GetClassAssignmentById(string classId = "", string studentId = "")
        {
            var exisitingCA = await _classAssignmentService.GetClassAssignmentByClassIdStudentId(classId, studentId);
            if (exisitingCA == null)
            {
                return BadRequest(new {Message = "Class assignment not found."});
            }

            return Ok(exisitingCA);
        }

        [HttpPost]
        public async Task<IActionResult> AddClassAssignment([FromBody] ClassAssignmentDTO caDTO)
        {
            if (caDTO == null)
            {
                return BadRequest(new {Message = "Invalid class assignment data."});
            }

            var ca = ModelMapUtils.MapBetweenClasses<ClassAssignmentDTO, ClassAssignment>(caDTO);

            await _classAssignmentService.AddAsync(ca);
            return CreatedAtAction(nameof(GetClassAssignmentById), new {ca.ClassId, ca.StudentId}, caDTO);
        }

        [HttpPut("{classId}/{studentId}")]
        public async Task<IActionResult> UpdateClassAssignment(string classId, string studentId, [FromBody] ClassAssignmentDTO caDTO)
        {
            if (classId == null || studentId == null || caDTO == null)
            {
                return BadRequest(new { Message = "Invalid class assignment data." });
            }

            var existingCa = await GetClassAssignmentById(classId, studentId);
            if (existingCa == null)
            {
                return BadRequest(new { Message = "Class assignment not found." });
            }

            caDTO.ClassId = classId;
            caDTO.StudentId = studentId;
            var ca = ModelMapUtils.MapBetweenClasses<ClassAssignmentDTO, ClassAssignment>(caDTO);

            await _classAssignmentService.UpdateAsync(ca);

            return NoContent();
        }

        [HttpDelete("{classId}/{studentId}")]
        public async Task<IActionResult> DeleteClassAssignment(string classId, string studentId)
        {
            var existingCa = await GetClassAssignmentById(classId, studentId);
            if (existingCa == null)
            {
                return BadRequest(new { Message = "Class assignment not found." });
            }

            await _classAssignmentService.DeleteAsync(existingCa);

            return NoContent();
        }
    }
}
