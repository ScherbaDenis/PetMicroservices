using Microsoft.AspNetCore.Mvc;
using Template.Domain.Services;

namespace WebApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        // GET: api/topic
        [HttpGet]
        public IActionResult GetAll()
        {
            // TODO: Implement GetAll logic using _topicService.GetAllAsync()
            return Ok();
        }

        // GET: api/topic/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // TODO: Implement GetById logic using _topicService.FindAsync(id)
            return Ok();
        }

        // POST: api/topic
        [HttpPost]
        public IActionResult Create([FromBody] object dto)
        {
            // TODO: Implement Create logic using _topicService.CreateAsync(dto)
            // Replace 'object' with appropriate DTO type when available
            return Ok();
        }

        // PUT: api/topic/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] object dto)
        {
            // TODO: Implement Update logic using _topicService.UpdateAsync(dto)
            // Replace 'object' with appropriate DTO type when available
            return Ok();
        }

        // DELETE: api/topic/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // TODO: Implement Delete logic using _topicService.DeleteAsync()
            // Retrieve entity first, then delete
            return Ok();
        }
    }
}
