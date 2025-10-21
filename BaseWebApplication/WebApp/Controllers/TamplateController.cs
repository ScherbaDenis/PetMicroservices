using Microsoft.AspNetCore.Mvc;
using Template.Domain.Services;
using Template.Domain.DTOs;

namespace WebApp.Controllers
{
    public class TamplatesController(ITamplateService service) : Controller
    {
        private readonly ITamplateService _service = service;

        // GET: /Tamplates
        public IActionResult Index()
        {
            var items = _service.GetAllAsync();
            return View(items);
        }

        // GET: /Tamplates/Details/{id}
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.FindAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Tamplates/Create
        public IActionResult Create() => View();

        // POST: /Tamplates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TamplateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(dto);
            await _service.CreateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tamplates/Edit/{id}
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.FindAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Tamplates/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TamplateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            await _service.UpdateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tamplates/Delete/{id}
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.FindAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Tamplates/Delete/{id}
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