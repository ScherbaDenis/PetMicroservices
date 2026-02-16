using Microsoft.AspNetCore.Mvc;
using Template.Domain.DTOs;
using Template.Domain.Services;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
        }

        // GET: api/question
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var questions = await _questionService.GetAllAsync(cancellationToken);
            return Ok(questions);
        }

        // GET: api/question/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var question = await _questionService.FindAsync(id, cancellationToken);
            
            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }

        // POST: api/question
        [HttpPost]
        public async Task<ActionResult<QuestionDto>> Create([FromBody] QuestionDto questionDto, CancellationToken cancellationToken = default)
        {
            if (questionDto == null)
            {
                return BadRequest("Question cannot be null");
            }

            var createdQuestion = await _questionService.CreateAsync(questionDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdQuestion.Id }, createdQuestion);
        }

        // PUT: api/question/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] QuestionDto questionDto, CancellationToken cancellationToken = default)
        {
            if (questionDto == null)
            {
                return BadRequest("Question cannot be null");
            }

            if (id != questionDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingQuestion = await _questionService.FindAsync(id, cancellationToken);
            if (existingQuestion == null)
            {
                return NotFound();
            }

            await _questionService.UpdateAsync(questionDto, cancellationToken);
            return NoContent();
        }

        // DELETE: api/question/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var question = await _questionService.FindAsync(id, cancellationToken);
            
            if (question == null)
            {
                return NotFound();
            }

            await _questionService.DeleteAsync(question, cancellationToken);
            return NoContent();
        }

        // DELETE: api/question/admin/{id} (Hard delete - for admin use only)
        [HttpDelete("admin/{id}")]
        public async Task<ActionResult> HardDelete(Guid id, CancellationToken cancellationToken = default)
        {
            var question = await _questionService.FindAsync(id, cancellationToken);
            
            if (question == null)
            {
                return NotFound();
            }

            await _questionService.HardDeleteAsync(question, cancellationToken);
            return NoContent();
        }

        // GET: api/question/admin/deleted (Get all deleted questions - for admin use only)
        [HttpGet("admin/deleted")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAllDeleted(CancellationToken cancellationToken = default)
        {
            var deletedQuestions = await _questionService.GetAllDeletedAsync(cancellationToken);
            return Ok(deletedQuestions);
        }

        // GET: api/question/admin/deleted/{id} (Get specific deleted question - for admin use only)
        [HttpGet("admin/deleted/{id}")]
        public async Task<ActionResult<QuestionDto>> GetDeletedById(Guid id, CancellationToken cancellationToken = default)
        {
            var question = await _questionService.FindDeletedAsync(id, cancellationToken);
            
            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }
    }
}
