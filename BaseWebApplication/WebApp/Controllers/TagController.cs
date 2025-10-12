using Microsoft.AspNetCore.Mvc;
using Template.Domain.Model;
using Template.Domain.Services;

namespace WebApp.Controllers
{
    public class TagController(ITagService service) : Controller
    {
        private readonly ITagService _service = service;

        // GET: /Tags
        public IActionResult Index()
        {
            var tags = _service.GetAllAsync();
            return View(tags);
        }

        //GET: /Tags/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var tag = await _service.FindAsync(id, cancellationToken);
            if (tag == null) return NotFound();
            return View(tag);
        }

        // GET: /Tags/Create
        public IActionResult Create() => View();

        // POST: /Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag tag, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(tag);
            await _service.CreateAsync(tag, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tags/Edit/5
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var tag = await _service.FindAsync(id, cancellationToken);
            if (tag == null) return NotFound();
            return View(tag);
        }

        // POST: /Tags/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tag tag, CancellationToken cancellationToken)
        {
            if (id != tag.Id) return BadRequest();
            if (!ModelState.IsValid) return View(tag);

            await _service.UpdateAsync(tag, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tags/Delete/5
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var tag = await _service.FindAsync(id, cancellationToken);
            if (tag == null) return NotFound();
            return View(tag);
        }

        // POST: /Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            var tag = await _service.FindAsync(id, cancellationToken);
            if (tag == null) return NotFound();

            await _service.DeleteAsync(tag, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}