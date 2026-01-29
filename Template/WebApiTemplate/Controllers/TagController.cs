using Microsoft.AspNetCore.Mvc;
using Template.Domain.DTOs;
using Template.Domain.Services;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
        }

        // GET: api/tag
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var tags = await _tagService.GetAllAsync(cancellationToken);
            return Ok(tags);
        }

        // GET: api/tag/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var tag = await _tagService.FindAsync(id, cancellationToken);
            
            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }

        // POST: api/tag
        [HttpPost]
        public async Task<ActionResult<TagDto>> Create([FromBody] TagDto tagDto, CancellationToken cancellationToken = default)
        {
            if (tagDto == null)
            {
                return BadRequest("Tag cannot be null");
            }

            await _tagService.CreateAsync(tagDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = tagDto.Id }, tagDto);
        }

        // PUT: api/tag/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] TagDto tagDto, CancellationToken cancellationToken = default)
        {
            if (tagDto == null)
            {
                return BadRequest("Tag cannot be null");
            }

            if (id != tagDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingTag = await _tagService.FindAsync(id, cancellationToken);
            if (existingTag == null)
            {
                return NotFound();
            }

            await _tagService.UpdateAsync(tagDto, cancellationToken);
            return NoContent();
        }

        // DELETE: api/tag/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var tag = await _tagService.FindAsync(id, cancellationToken);
            
            if (tag == null)
            {
                return NotFound();
            }

            await _tagService.DeleteAsync(tag, cancellationToken);
            return NoContent();
        }
    }
}
