using Microsoft.AspNetCore.Mvc;
using Template.Domain.Services;
using Template.Domain.DTOs;

namespace WebApp.Controllers
{
    public class TemplatesController(ITemplateService service) : Controller
    {
        private readonly ITemplateService _service = service;

        // GET: /Templates
        public IActionResult Index()
        {
            var items = _service.GetAllAsync();
            return View(items);
        }

        // GET: /Templates/Details/{id}
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.FindAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Templates/Create
        public IActionResult Create() => View();

        // POST: /Templates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TemplateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(dto);
            await _service.CreateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Templates/Edit/{id}
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.FindAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Templates/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TemplateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            await _service.UpdateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Templates/Delete/{id}
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.FindAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Templates/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.FindAsync(id, cancellationToken);
            if (item == null) return NotFound();

            await _service.DeleteAsync(item, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}