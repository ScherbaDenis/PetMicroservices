using Microsoft.AspNetCore.Mvc;
using Template.Domain.DTOs;
using Template.Domain.Services;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITemplateService _templateService;

        public UserController(IUserService userService, ITemplateService templateService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }

        // GET: api/user
        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var users = _userService.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.FindAsync(id, cancellationToken);
            
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserDto userDto, CancellationToken cancellationToken = default)
        {
            if (userDto == null)
            {
                return BadRequest("User cannot be null");
            }

            await _userService.CreateAsync(userDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = userDto.Id }, userDto);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UserDto userDto, CancellationToken cancellationToken = default)
        {
            if (userDto == null)
            {
                return BadRequest("User cannot be null");
            }

            if (id != userDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingUser = await _userService.FindAsync(id, cancellationToken);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userService.UpdateAsync(userDto, cancellationToken);
            return NoContent();
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.FindAsync(id, cancellationToken);
            
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(user, cancellationToken);
            return NoContent();
        }

        // GET: api/user/{id}/templates
        [HttpGet("{id}/templates")]
        public async Task<ActionResult<IEnumerable<TemplateDto>>> GetTemplates(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.FindAsync(id, cancellationToken);
            
            if (user == null)
            {
                return NotFound();
            }

            var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
            return Ok(templates);
        }
    }
}
