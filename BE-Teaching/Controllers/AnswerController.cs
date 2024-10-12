using BE_Teaching.Models;
using BE_Teaching.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_Teaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        public AnswerController(IAnswerService answer)
        {
            _answerService = answer;
        }

        [HttpGet("AnswerList")]
        public IActionResult GetAllAnswers()
        {
            try
            {
                var response = _answerService.GetAllAnswers();

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

        [HttpPost("AddAnswer")]
        public IActionResult AddAnswer(Answer newAnswer)
        {
            {
                if (newAnswer == null)
                {
                    return BadRequest("Answer data is required.");
                }
                try
                {
                    var response = _answerService.AddAnswer(newAnswer);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut("EditAnswer")]
        public IActionResult EditAnswer(int id, Answer editAnswerr)
        {
            if (id == null || editAnswerr == null)
            {
                return BadRequest("Exam data is required.");
            }
            try
            {
                var response = _answerService.EditAnswer(id, editAnswerr);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteAnswer")]
        public IActionResult DeleteAnswer(int id)
        {
            if (id == null)
            {
                return BadRequest("AnswerID is required");
            }
            try
            {
                var response = _answerService.DeleteAnswer(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
