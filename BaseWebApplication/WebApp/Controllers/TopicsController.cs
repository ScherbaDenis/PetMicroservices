using Microsoft.AspNetCore.Mvc;
using Template.Domain.DTOs;
using Template.Domain.Services;

namespace WebApp.Controllers
{
    public class TopicsController(ITopicService service) : Controller
    {
        private readonly ITopicService _service = service;

        // GET: /Topics
        public IActionResult Index()
        {
            var topics = _service.GetAllAsync();
            return View(topics);
        }

        // GET: /Topics/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var topic = await _service.FindAsync(id, cancellationToken);
            if (topic == null) return NotFound();
            return View(topic);
        }

        // GET: /Topics/Create
        public IActionResult Create() => View();

        // POST: /Topics/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopicDto topic, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(topic);
            await _service.CreateAsync(topic, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Topics/Edit/5
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var topic = await _service.FindAsync(id, cancellationToken);
            if (topic == null) return NotFound();
            return View(topic);
        }

        // POST: /Topics/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TopicDto topic, CancellationToken cancellationToken)
        {
            if (id != topic.Id) return BadRequest();
            if (!ModelState.IsValid) return View(topic);

            await _service.UpdateAsync(topic, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Topics/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var topic = await _service.FindAsync(id, cancellationToken);
            if (topic == null) return NotFound();
            return View(topic);
        }

        // POST: /Topics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var topic = await _service.FindAsync(id, cancellationToken);
            if (topic == null) return NotFound();

            await _service.DeleteAsync(topic, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
