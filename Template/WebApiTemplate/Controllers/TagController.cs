using Microsoft.AspNetCore.Mvc;
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
            _tagService = tagService;
        }

        // GET: api/tag
        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Implement GetAll logic using _tagService.GetAllAsync()
            return Ok();
        }

        // GET: api/tag/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // TODO: Implement GetById logic using _tagService.FindAsync(id)
            return Ok();
        }

        // POST: api/tag
        [HttpPost]
        public IActionResult Create([FromBody] object dto)
        {
            // TODO: Implement Create logic using _tagService.CreateAsync(dto)
            // Replace 'object' with appropriate DTO type when available
            return Ok();
        }

        // PUT: api/tag/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object dto)
        {
            // TODO: Implement Update logic using _tagService.UpdateAsync(dto)
            // Replace 'object' with appropriate DTO type when available
            return Ok();
        }

        // DELETE: api/tag/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // TODO: Implement Delete logic using _tagService.DeleteAsync()
            // Retrieve entity first, then delete
            return Ok();
        }
    }
}
