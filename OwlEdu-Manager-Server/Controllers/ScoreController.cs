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
    public class ScoreController : Controller
    {
        private readonly ScoreService _scoreService;
        public ScoreController(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }
        // GET: api/Score/all
        [HttpGet]
        public async Task<IActionResult> GetAllScores(int pageNumber = 1, int pageSize = 10)
        {
            var scores = await _scoreService.GetAllAsync(pageNumber, pageSize, "Title");
            var response = scores.Select(score => ModelMapUtils.MapBetweenClasses<Score, ScoreResponse>(score));
            return Ok(response);
        }
        // GET: api/Score/Class/{classId}
        [HttpGet("Class/{classId}")]
        public async Task<IActionResult> GetScoresByClass(string classId)
        {
            var scores = await _scoreService.GetScoresByClassAsync(classId);
            var response = scores.Select(score => ModelMapUtils.MapBetweenClasses<Score, ScoreResponse>(score));
            return Ok(response);
        }

        // GET: api/Score/by-student/{studentId}
        [HttpGet("Student/{studentId}")]
        public async Task<IActionResult> GetScoresByStudent(string studentId)
        {
            var scores = await _scoreService.GetScoresByStudentAsync(studentId);
            var response = scores.Select(score => ModelMapUtils.MapBetweenClasses<Score, ScoreResponse>(score));
            return Ok(response);
        }

        // GET: api/Score/by-teacher/{teacherId}
        [HttpGet("Teacher/{teacherId}")]
        public async Task<IActionResult> GetScoresByTeacher(string teacherId)
        {
            var scores = await _scoreService.GetScoresByTeacherAsync(teacherId);
            var response = scores.Select(score => ModelMapUtils.MapBetweenClasses<Score, ScoreResponse>(score));
            return Ok(response);
        }

        // POST: api/Score
        [HttpPost]
        public async Task<IActionResult> AddScore([FromBody] ScoreRequest scoreRequest)
        {
            if (scoreRequest == null)
            {
                return BadRequest(new { Message = "Invalid score data." });
            }

            var score = ModelMapUtils.MapBetweenClasses<ScoreRequest, Score>(scoreRequest);
            score.CreatedAt = DateTime.UtcNow;
            score.UpdatedAt = DateTime.UtcNow;

            await _scoreService.AddAsync(score);

            var response = ModelMapUtils.MapBetweenClasses<Score, ScoreResponse>(score);
            return CreatedAtAction(nameof(GetScoresByStudent), new { studentId = score.StudentId }, response);
        }

        // PUT: api/Score
        [HttpPut("{studentId}/{classId}/{teacherId}/{title}")]
        public async Task<IActionResult> UpdateScore(string studentId, string classId, string teacherId, string title, [FromBody] ScoreRequestUPDATE scoreRequest)
        {
            if (scoreRequest == null)
            {
                return BadRequest(new { Message = "Invalid score data." });
            }

            var existingScores = await _scoreService.FindAsync(
                s => s.StudentId == studentId &&
                     s.ClassId == classId &&
                     s.TeacherId == teacherId &&
                     s.Title == title, 1, 1);

            if (!existingScores.Any())
            {
                return NotFound(new { Message = "Score not found." });
            }
            var score = existingScores.First();
            //score.TeacherId = scoreRequest.TeacherId;
            score.Lisening = scoreRequest.Lisening;
            score.Speaking = scoreRequest.Speaking;
            score.Reading = scoreRequest.Reading;
            score.Writing = scoreRequest.Writing;
            score.Type = scoreRequest.Type;
            score.UpdatedAt = DateTime.UtcNow;
            await _scoreService.UpdateAsync(score);
            var response = ModelMapUtils.MapBetweenClasses<Score, ScoreRequestUPDATE>(score);
            return Ok(response);
        }

        // DELETE: api/Score/{studentId}/{classId}/{teacherId}/{title}
        [HttpDelete("{studentId}/{classId}/{teacherId}/{title}")]
        public async Task<IActionResult> DeleteScore(string studentId, string classId, string teacherId, string title)
        {
            var existingScores = await _scoreService.FindAsync(
                s => s.StudentId == studentId &&
                     s.ClassId == classId &&
                     s.TeacherId == teacherId &&
                     s.Title == title, 1, 1);

            if (!existingScores.Any())
            {
                return NotFound(new { Message = "Score not found." });
            }

            var score = existingScores.First();
            await _scoreService.DeleteAsync(score);

            return NoContent();
        }
    }
}
