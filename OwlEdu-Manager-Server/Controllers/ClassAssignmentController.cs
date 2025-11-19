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

            var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IEnumerable<ClassAssignment> finalResult = null;

            foreach (var key in keywords)
            {
                // Tìm theo từng loại
                var byStr = await _classAssignmentService.GetByStringKeywordAsync(key, -1, pageSize, "ClassId", "StudentId");
                var byDate = await _classAssignmentService.GetByDateTimeKeywordAsync(key, -1, pageSize, "ClassId", "StudentId");

                // Ghép kết quả của TỪ khóa hiện tại
                var unionForCurrentKeyword = byStr
                    .Concat(byDate)
                    .DistinctBy(t => new { t.StudentId, t.ClassId });

                // Lần đầu → gán luôn
                if (finalResult == null)
                    finalResult = unionForCurrentKeyword;
                else
                    // Giao nhau để chỉ giữ các item match “mọi keyword”
                    finalResult = finalResult.Intersect(unionForCurrentKeyword);
            }

            // Map sang DTO
            var res = finalResult.Select(t => ModelMapUtils.MapBetweenClasses<ClassAssignment, ClassAssignmentDTO>(t)).ToList();

            if (res == null) res = new List<ClassAssignmentDTO>();

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
        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetClassAssignmentByClassId(string classId)
        {
            if (string.IsNullOrWhiteSpace(classId))
            {
                return BadRequest(new { Message = "Class ID is required." });
            }

            var classAssignments = await _classAssignmentService.GetClassAssignmentByClassId(classId);
            if (!classAssignments.Any())
            {
                return NotFound(new { Message = "No class assignments found for the given class ID." });
            }

            var result = classAssignments.Select(t => ModelMapUtils.MapBetweenClasses<ClassAssignment, ClassAssignmentDTO>(t)).ToList();
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetClassAssignmentByStudentId(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                return BadRequest(new { Message = "Student ID is required." });
            }

            var classAssignments = await _classAssignmentService.GetClassAssignmentByStudentId(studentId);
            if (!classAssignments.Any())
            {
                return NotFound(new { Message = "No class assignments found for the given student ID." });
            }

            var result = classAssignments.Select(t => ModelMapUtils.MapBetweenClasses<ClassAssignment, ClassAssignmentDTO>(t)).ToList();
            return Ok(result);
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
            return CreatedAtAction(nameof(GetClassAssignmentById), new {ca.ClassId, ca.StudentId}, ModelMapUtils.MapBetweenClasses<ClassAssignment, ClassAssignmentDTO>(ca));
        }

        [HttpPut("{classId}/{studentId}")]
        public async Task<IActionResult> UpdateClassAssignment(string classId, string studentId, [FromBody] ClassAssignmentDTO caDTO)
        {
            if (classId == null || studentId == null || caDTO == null)
            {
                return BadRequest(new { Message = "Invalid class assignment data." });
            }

            var existingCa = await _classAssignmentService.GetClassAssignmentByClassIdStudentId(classId, studentId);
            if (existingCa == null)
            {
                return BadRequest(new { Message = "Class assignment not found." });
            }

            existingCa.AssignedDate = caDTO.AssignedDate;
            existingCa.Status = caDTO.Status;

            await _classAssignmentService.UpdateAsync(existingCa);

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

            await _classAssignmentService.DeleteClassAssigment(classId, studentId);

            return NoContent();
        }
    }
}
