using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetMicroservices.Comments.Dtos;
using PetMicroservices.Comments.Models;
using PetMicroservices.Comments.Repositories;

namespace PetMicroservices.WebApiComment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _repository;

        public CommentController(ICommentRepository repository)
        {
            _repository = repository;
        }

        // GET api/comment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetAll()
        {
            var comments = await _repository.GetAllAsync();
            var dtos = comments.Select(c => ToDto(c));
            return Ok(dtos);
        }

        // GET api/comment/{id}
        [HttpGet("{id:int}", Name = nameof(GetById))]
        public async Task<ActionResult<CommentDto>> GetById(int id)
        {
            var comment = await _repository.GetByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(ToDto(comment));
        }

        // GET api/comment/post/{postId}
        [HttpGet("post/{postId:int}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetByPostId(int postId)
        {
            var comments = await _repository.GetByPostIdAsync(postId);
            var dtos = comments.Select(c => ToDto(c));
            return Ok(dtos);
        }

        // POST api/comment
        [HttpPost]
        public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var comment = new Comment
            {
                PostId = createDto.PostId,
                Author = createDto.Author,
                Text = createDto.Text
            };

            var created = await _repository.CreateAsync(comment);
            var dto = ToDto(created);

            return CreatedAtRoute(nameof(GetById), new { id = dto.Id }, dto);
        }

        // DELETE api/comment/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _repository.GetByIdAsync(id);
            if (exists == null) return NotFound();

            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // PUT api/comment/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCommentDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Author = updateDto.Author;
            existing.Text = updateDto.Text;
            existing.PostId = updateDto.PostId;

            await _repository.UpdateAsync(existing);

            return NoContent();
        }

        private static CommentDto ToDto(Comment c) =>
            new CommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                Author = c.Author,
                Text = c.Text,
                CreatedAt = c.CreatedAt
            };
    }
}