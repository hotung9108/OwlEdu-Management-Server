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
                return Ok(await _classAssignmentService.GetAllAsync(pageNumber, pageSize));
            }

            var classAssignmentByString = _classAssignmentService.GetByStringKeywordAsync(keyword, pageNumber, pageSize);
            var classAssignmentByDateTime = _classAssignmentService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize);

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

            var existingCa = await GetClassAssignmentById(caDTO.ClassId, caDTO.StudentId);
            if (existingCa !=  null)
            {
                return BadRequest(new { Message = "Invalid class assignment data." });
            }

            var ca = ModelMapUtils.MapBetweenClasses<ClassAssignmentDTO, ClassAssignment>(caDTO);

            await _classAssignmentService.AddAsync(ca);
            return Ok(ca);
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

            var ca = ModelMapUtils.MapBetweenClasses<ClassAssignmentDTO, ClassAssignment>(caDTO);

            await _classAssignmentService.UpdateAsync(ca);

            return NoContent();
        }

        [HttpDelete("{classId}/{studentId}")]
        public async Task<IActionResult> DeleteClassAssignment(string classId, string studentId, [FromBody] ClassAssignmentDTO caDTO)
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

            var ca = ModelMapUtils.MapBetweenClasses<ClassAssignmentDTO, ClassAssignment>(caDTO);

            await _classAssignmentService.DeleteClassAssignment(classId, studentId);

            return NoContent();
        }
    }
}
