using BE_Teaching.Models;
using BE_Teaching.Services;
using Microsoft.AspNetCore.Mvc;

namespace BE_Teaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService question)
        {
            _questionService = question;
        }

        [HttpGet("QuestionList")]
        public IActionResult GetAllQuestion()
        {
            try
            {
                var response = _questionService.GetAllQuestions();

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

        [HttpPost("AddQuestion")]
        public IActionResult AddQuestion(Question newQuestion)
        {
            {
                if (newQuestion == null)
                {
                    return BadRequest("Question data is required.");
                }
                try
                {
                    var response = _questionService.AddQuestion(newQuestion);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut("EditQuestion")]
        public IActionResult EditQuestion(int id, Question editQuestion)
        {
            if (id == null || editQuestion == null)
            {
                return BadRequest("Exam data is required.");
            }
            try
            {
                var response = _questionService.EditQuestion(id, editQuestion);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteQuestion")]
        public IActionResult DeleteQuestion(int id)
        {
            if (id == null)
            {
                return BadRequest("QuestionID is required");
            }
            try
            {
                var response = _questionService.DeleteQuestion(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
