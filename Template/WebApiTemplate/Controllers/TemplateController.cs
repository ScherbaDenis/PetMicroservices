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
        private readonly IUserService _userService;

        public TemplateController(ITemplateService templateService, IUserService userService)
        {
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // GET: api/template
        [HttpGet]
        public async Task< ActionResult<IEnumerable<TemplateDto>>> GetAll(CancellationToken cancellationToken = default)
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

            var createdTemplate = await _templateService.CreateAsync(templateDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdTemplate.Id }, createdTemplate);
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

        // GET: api/template/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TemplateDto>>> GetByUserId(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userService.FindAsync(userId, cancellationToken);
            
            if (user == null)
            {
                return NotFound();
            }

            var templates = await _templateService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(templates);
        }

        // POST: api/template/{templateId}/assign/{userId}
        [HttpPost("{templateId}/assign/{userId}")]
        public async Task<ActionResult> AssignTemplateToUser(Guid templateId, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _templateService.AssignTemplateToUserAsync(templateId, userId, cancellationToken);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/template/{templateId}/assign/{userId}
        [HttpDelete("{templateId}/assign/{userId}")]
        public async Task<ActionResult> UnassignTemplateFromUser(Guid templateId, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _templateService.UnassignTemplateFromUserAsync(templateId, userId, cancellationToken);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
