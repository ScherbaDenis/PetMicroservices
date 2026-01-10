using Microsoft.AspNetCore.Mvc;
using Template.Domain.DTOs;
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
            _topicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
        }

        // GET: api/topic
        [HttpGet]
        public ActionResult<IEnumerable<TopicDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var topics = _topicService.GetAllAsync(cancellationToken);
            return Ok(topics);
        }

        // GET: api/topic/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDto>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var topic = await _topicService.FindAsync(id, cancellationToken);
            
            if (topic == null)
            {
                return NotFound();
            }

            return Ok(topic);
        }

        // POST: api/topic
        [HttpPost]
        public async Task<ActionResult<TopicDto>> Create([FromBody] TopicDto topicDto, CancellationToken cancellationToken = default)
        {
            if (topicDto == null)
            {
                return BadRequest("Topic cannot be null");
            }

            await _topicService.CreateAsync(topicDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = topicDto.Id }, topicDto);
        }

        // PUT: api/topic/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] TopicDto topicDto, CancellationToken cancellationToken = default)
        {
            if (topicDto == null)
            {
                return BadRequest("Topic cannot be null");
            }

            if (id != topicDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingTopic = await _topicService.FindAsync(id, cancellationToken);
            if (existingTopic == null)
            {
                return NotFound();
            }

            await _topicService.UpdateAsync(topicDto, cancellationToken);
            return NoContent();
        }

        // DELETE: api/topic/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var topic = await _topicService.FindAsync(id, cancellationToken);
            
            if (topic == null)
            {
                return NotFound();
            }

            await _topicService.DeleteAsync(topic, cancellationToken);
            return NoContent();
        }
    }
}
