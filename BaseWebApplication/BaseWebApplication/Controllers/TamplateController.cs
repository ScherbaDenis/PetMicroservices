using Microsoft.AspNetCore.Mvc;
using Template.Domain.Model;
using Template.Domain.Services;

namespace BaseWebApplication.Controllers
{
    public class TamplatesController(ITamplateService service) : Controller
    {
        private readonly ITamplateService _service = service;

        // GET: /Tamplates
        public IActionResult Index()
        {
            var tamplates = _service.GetAllAsync();
            return View(tamplates);
        }

        // GET: /Tamplates/Details/5
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var tamplate = await _service.FindAsync(id, cancellationToken);
            if (tamplate == null) return NotFound();
            return View(tamplate);
        }

        // GET: /Tamplates/Create
        public IActionResult Create() => View();

        // POST: /Tamplates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tamplate tamplate, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(tamplate);
            await _service.CreateAsync(tamplate, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tamplates/Edit/5
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var tamplate = await _service.FindAsync(id, cancellationToken);
            if (tamplate == null) return NotFound();
            return View(tamplate);
        }

        // POST: /Tamplates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Tamplate tamplate, CancellationToken cancellationToken)
        {
            if (id != tamplate.Id) return BadRequest();
            if (!ModelState.IsValid) return View(tamplate);

            await _service.UpdateAsync(tamplate, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tamplates/Delete/5
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var tamplate = await _service.FindAsync(id, cancellationToken);
            if (tamplate == null) return NotFound();
            return View(tamplate);
        }

        // POST: /Tamplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
        {
            var tamplate = await _service.FindAsync(id, cancellationToken);
            if (tamplate == null) return NotFound();

            await _service.DeleteAsync(tamplate, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}