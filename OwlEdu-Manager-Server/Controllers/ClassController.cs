using Microsoft.AspNetCore.Mvc;
using OwlEdu_Manager_Server.DTOs;
using OwlEdu_Manager_Server.Models;
using OwlEdu_Manager_Server.Services;
using OwlEdu_Manager_Server.Utils;

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

        //GET: api/Class
        [HttpGet]
        public async Task<IActionResult> GetAllClasses([FromQuery] string keyword = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (keyword.Trim() == "")
            {
                var classes = await _classService.GetAllAsync(pageNumber, pageSize, "Id");
                return Ok(classes.Select(ModelMapUtils.MapBetweenClasses<Class, ClassDTO>).ToList());
            }

            var classesByString = _classService.GetByStringKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var classesByNumeric = _classService.GetByNumericKeywordAsync(keyword, pageNumber, pageSize, "Id");
            var classesByDateTime = _classService.GetByDateTimeKeywordAsync(keyword, pageNumber, pageSize, "Id");

            await Task.WhenAll(classesByString, classesByNumeric, classesByDateTime);

            var res = classesByString.Result.Concat(classesByNumeric.Result).Concat(classesByDateTime.Result).DistinctBy(t => t.Id).Select(ModelMapUtils.MapBetweenClasses<Class, ClassDTO>).ToList();
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

            if (classes == null)
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
            return CreatedAtAction(nameof(GetClassById), new {id = _class.Id}, classDTO);
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

            classDTO.Id = id;
            var _class = ModelMapUtils.MapBetweenClasses<ClassDTO, Class>(classDTO);

            await _classService.UpdateAsync(_class);
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

            await _classService.DeleteAsync(existingClass);
            return NoContent();
        }
    }
}
