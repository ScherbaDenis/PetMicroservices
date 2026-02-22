using Microsoft.AspNetCore.Mvc;
using Template.Domain.DTOs;
using Template.Service.Services;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // GET: api/user
        [HttpGet]
        public async Task< ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
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

            var createdUser = await _userService.CreateAsync(userDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
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

        // DELETE: api/user/admin/{id} (Hard delete - for admin use only)
        [HttpDelete("admin/{id}")]
        public async Task<ActionResult> HardDelete(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.FindAsync(id, cancellationToken);
            
            if (user == null)
            {
                return NotFound();
            }

            await _userService.HardDeleteAsync(user, cancellationToken);
            return NoContent();
        }

        // GET: api/user/admin/deleted (Get all deleted users - for admin use only)
        [HttpGet("admin/deleted")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllDeleted(CancellationToken cancellationToken = default)
        {
            var deletedUsers = await _userService.GetAllDeletedAsync(cancellationToken);
            return Ok(deletedUsers);
        }

        // GET: api/user/admin/deleted/{id} (Get specific deleted user - for admin use only)
        [HttpGet("admin/deleted/{id}")]
        public async Task<ActionResult<UserDto>> GetDeletedById(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.FindDeletedAsync(id, cancellationToken);
            
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
