using Comment.Domain.DTOs;
using Comment.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApiComment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }

        // GET: api/template
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemplateDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var templates = await _templateService.GetAllAsync(cancellationToken);
            return Ok(templates);
        }

        // GET: api/template/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TemplateDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var template = await _templateService.FindAsync(id, cancellationToken);
            
            if (template == null)
            {
                return NotFound();
            }

            return Ok(template);
        }

        // POST: api/template
        [HttpPost]
        public async Task<ActionResult<TemplateDto>> Create([FromBody] TemplateDto templateDto, CancellationToken cancellationToken = default)
        {
            if (templateDto == null)
            {
                return BadRequest("Template cannot be null");
            }

            await _templateService.CreateAsync(templateDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = templateDto.Id }, templateDto);
        }

        // PUT: api/template/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] TemplateDto templateDto, CancellationToken cancellationToken = default)
        {
            if (templateDto == null)
            {
                return BadRequest("Template cannot be null");
            }

            if (id != templateDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingTemplate = await _templateService.FindAsync(id, cancellationToken);
            if (existingTemplate == null)
            {
                return NotFound();
            }

            await _templateService.UpdateAsync(templateDto, cancellationToken);
            return NoContent();
        }

        // DELETE: api/template/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var template = await _templateService.FindAsync(id, cancellationToken);
            
            if (template == null)
            {
                return NotFound();
            }

            await _templateService.DeleteAsync(template, cancellationToken);
            return NoContent();
        }

        // DELETE: api/template/admin/{id} (Hard delete - for admin use only)
        [HttpDelete("admin/{id}")]
        public async Task<ActionResult> HardDelete(Guid id, CancellationToken cancellationToken = default)
        {
            var template = await _templateService.FindAsync(id, cancellationToken);
            
            if (template == null)
            {
                return NotFound();
            }

            await _templateService.HardDeleteAsync(template, cancellationToken);
            return NoContent();
        }

        // GET: api/template/admin/deleted (Get all deleted templates - for admin use only)
        [HttpGet("admin/deleted")]
        public async Task<ActionResult<IEnumerable<TemplateDto>>> GetAllDeleted(CancellationToken cancellationToken = default)
        {
            var deletedTemplates = await _templateService.GetAllDeletedAsync(cancellationToken);
            return Ok(deletedTemplates);
        }

        // GET: api/template/admin/deleted/{id} (Get specific deleted template - for admin use only)
        [HttpGet("admin/deleted/{id}")]
        public async Task<ActionResult<TemplateDto>> GetDeletedById(Guid id, CancellationToken cancellationToken = default)
        {
            var template = await _templateService.FindDeletedAsync(id, cancellationToken);
            
            if (template == null)
            {
                return NotFound();
            }

            return Ok(template);
        }
    }
}
