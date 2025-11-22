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
    public class ClassController : ControllerBase
    {
        private readonly ClassService _classService;

        public ClassController(ClassService classService)
        {
            _classService = classService;
        }

        //GET: api/Class
        [HttpGet]
        public async Task<IActionResult> GetAllClasses([FromQuery] string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (keyword.Trim() == "")
            {
                var classes = await _classService.GetAllAsync(pageNumber, pageSize, "Id");
                return Ok(classes.Select(ModelMapUtils.MapBetweenClasses<Class, ClassDTO>).ToList());
            }
            var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IEnumerable<Class> finalResult = null;

            foreach (var key in keywords)
            {
                // Tìm theo từng loại
                var byStr = await _classService.GetByStringKeywordAsync(key, -1, pageSize, "Id");
                var byNum = await _classService.GetByNumericKeywordAsync(key, -1, pageSize, "Id");
                var byDate = await _classService.GetByDateTimeKeywordAsync(key, -1, pageSize, "Id");

                // Ghép kết quả của TỪ khóa hiện tại
                var unionForCurrentKeyword = byStr
                    .Concat(byNum)
                    .Concat(byDate)
                    .DistinctBy(t => t.Id);

                // Lần đầu → gán luôn
                if (finalResult == null)
                    finalResult = unionForCurrentKeyword;
                else
                    // Giao nhau để chỉ giữ các item match “mọi keyword”
                    finalResult = finalResult.Intersect(unionForCurrentKeyword);
            }

            // Map sang DTO
            var res = finalResult.Select(ModelMapUtils.MapBetweenClasses<Class, ClassDTO>).ToList();
            if (pageNumber != -1)
            {
                res = finalResult.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(ModelMapUtils.MapBetweenClasses<Class, ClassDTO>).ToList();
            }

            if (res == null) res = new List<ClassDTO>();

            return Ok(res);
        }
        //GET: api/Class/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassById(string id)
        {
            var _class = await _classService.GetByIdAsync(id);
            if (_class == null)
            {
                return BadRequest(new { Message = "Class not found." });
            }
            var res = ModelMapUtils.MapBetweenClasses<Class, ClassDTO>(_class);
            return Ok(res);
        }
        //POST api/Class
        [HttpPost]
        public async Task<IActionResult> AddClass([FromBody] ClassDTO classDTO)
        {
            if (classDTO == null)
            {
                return BadRequest(new { Message = "Invalid class data."});
            }

            var _class = ModelMapUtils.MapBetweenClasses<ClassDTO, Class>(classDTO);

            var classes = await _classService.GetAllAsync(-1, -1, "Id");

            if (classes.Count() == 0)
            {
                _class.Id = "LH000000";
            }
            else
            {
                var last = classes.Last();

                int lastestIdNumber = int.Parse(last.Id.Substring(2));

                int newIdNumber = lastestIdNumber + 1;

                string newId = "LH";

                for (int i = 0; i < 6 - newIdNumber.ToString().Length; i++) newId += "0";

                _class.Id = newId + newIdNumber.ToString();
            }

            await _classService.AddAsync(_class);
            return CreatedAtAction(nameof(GetClassById), new {id = _class.Id}, ModelMapUtils.MapBetweenClasses<Class, ClassDTO>(_class));
        }
        //PUT: api/Class/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(string id, [FromBody] ClassDTO classDTO)
        {
            if (id == null || classDTO == null)
            {
                return BadRequest(new { Message = "Invalid class data."});
            }

            var existingClass = await _classService.GetByIdAsync(id);
            if (existingClass == null)
            {
                return BadRequest(new {Message = "Class not found." });
            }

            existingClass.CourseId = classDTO.CourseId;
            existingClass.Status = classDTO.Status;
            existingClass.Name = classDTO.Name;
            existingClass.Require = classDTO.Require;
            existingClass.Target = classDTO.Target;
            existingClass.MaxStudents = classDTO.MaxStudents;
            existingClass.StartDate = classDTO.StartDate;
            existingClass.EndDate = classDTO.EndDate;
            existingClass.TeacherId = classDTO.TeacherId;

            await _classService.UpdateAsync(existingClass);
            return NoContent();
        }
        //DELETE: api/Class/{id}
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

        [HttpPatch("status/{id}/{status}")]
        public async Task<IActionResult> PatchClassStatus(string id, bool status)
        {
            if (id == null)
            {
                return BadRequest(new { Message = "Invalid class data." });
            }

            var existingClass = await _classService.GetByIdAsync(id);
            if (existingClass == null)
            {
                return BadRequest(new { Message = "Class not found." });
            }

            existingClass.Status = status;

            await _classService.UpdateAsync(existingClass);
            return NoContent();
        }
    }
}
