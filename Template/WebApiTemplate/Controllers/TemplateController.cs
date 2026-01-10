using Microsoft.AspNetCore.Mvc;
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
            _templateService = templateService;
        }

        // GET: api/template
        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Implement GetAll logic using _templateService.GetAllAsync()
            return Ok();
        }

        // GET: api/template/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // TODO: Implement GetById logic using _templateService.FindAsync(id)
            return Ok();
        }

        // POST: api/template
        [HttpPost]
        public IActionResult Create([FromBody] object dto)
        {
            // TODO: Implement Create logic using _templateService.CreateAsync(dto)
            // Replace 'object' with appropriate DTO type when available
            return Ok();
        }

        // PUT: api/template/{id}
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] object dto)
        {
            // TODO: Implement Update logic using _templateService.UpdateAsync(dto)
            // Replace 'object' with appropriate DTO type when available
            return Ok();
        }

        // DELETE: api/template/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            // TODO: Implement Delete logic using _templateService.DeleteAsync()
            // Retrieve entity first, then delete
            return Ok();
        }
    }
}
