using Microsoft.AspNetCore.Mvc;
using Template.Domain.Services;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Implement GetAll logic using _userService.GetAllAsync()
            return Ok();
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // TODO: Implement GetById logic using _userService.FindAsync(id)
            return Ok();
        }

        // POST: api/user
        [HttpPost]
        public IActionResult Create([FromBody] object dto)
        {
            // TODO: Implement Create logic using _userService.CreateAsync(dto)
            // Replace 'object' with appropriate DTO type when available
            return Ok();
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] object dto)
        {
            // TODO: Implement Update logic using _userService.UpdateAsync(dto)
            // Replace 'object' with appropriate DTO type when available
            return Ok();
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            // TODO: Implement Delete logic using _userService.DeleteAsync()
            // Retrieve entity first, then delete
            return Ok();
        }
    }
}
