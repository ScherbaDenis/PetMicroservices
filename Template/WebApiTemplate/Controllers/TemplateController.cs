using Microsoft.AspNetCore.Mvc;
using Template.Domain.DTOs;
using Template.Domain.Services;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }

        // GET: api/template
        [HttpGet]
        public ActionResult<IEnumerable<TemplateDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var templates = _templateService.GetAllAsync(cancellationToken);
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
    }
}
