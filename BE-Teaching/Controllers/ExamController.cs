using BE_Teaching.Models;
using BE_Teaching.Services;
using Microsoft.AspNetCore.Mvc;

namespace BE_Teaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet("ExamList")]
        public IActionResult GetAllExam()
        {
            try
            {
                var response = _examService.GetAllExams();

                if (response.Data == null || !response.Data.Any())
                {
                    return NoContent();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("AddExam")]
        public IActionResult AddExam(Exam newExam)
        {
            {
                if (newExam == null)
                {
                    return BadRequest("Exam data is required.");
                }
                try
                {
                    var response = _examService.AddExam(newExam);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut("EditExam")]
        public IActionResult EditExam(int id, Exam editExam)
        {
            if (id == null || editExam == null)
            {
                return BadRequest("Exam data is required.");
            }
            try
            {
                var response = _examService.EditExam(id, editExam);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteExam")]
        public IActionResult DeleteExam(int id)
        {
            if (id == null)
            {
                return BadRequest("ExamID is required");
            }
            try
            {
                var response = _examService.DeleteExam(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
