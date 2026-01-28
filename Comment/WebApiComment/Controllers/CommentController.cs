using Comment.Domain.DTOs;
using Comment.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApiComment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        // GET: api/comment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var comments = await _commentService.GetAllAsync(cancellationToken);
            return Ok(comments);
        }

        // GET: api/comment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var comment = await _commentService.FindAsync(id, cancellationToken);
            
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // GET: api/comment/template/{templateId}
        [HttpGet("template/{templateId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetByTemplateId(Guid templateId, CancellationToken cancellationToken = default)
        {
            var comments = await _commentService.GetByTemplateAsync(templateId, cancellationToken);
            return Ok(comments);
        }

        // POST: api/comment
        [HttpPost]
        public async Task<ActionResult<CommentDto>> Create([FromBody] CommentDto commentDto, CancellationToken cancellationToken = default)
        {
            if (commentDto == null)
            {
                return BadRequest("Comment cannot be null");
            }

            await _commentService.CreateAsync(commentDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = commentDto.Id }, commentDto);
        }

        // PUT: api/comment/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] CommentDto commentDto, CancellationToken cancellationToken = default)
        {
            if (commentDto == null)
            {
                return BadRequest("Comment cannot be null");
            }

            if (id != commentDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingComment = await _commentService.FindAsync(id, cancellationToken);
            if (existingComment == null)
            {
                return NotFound();
            }

            await _commentService.UpdateAsync(commentDto, cancellationToken);
            return NoContent();
        }

        // DELETE: api/comment/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var comment = await _commentService.FindAsync(id, cancellationToken);
            
            if (comment == null)
            {
                return NotFound();
            }

            await _commentService.DeleteAsync(comment, cancellationToken);
            return NoContent();
        }
    }
}
