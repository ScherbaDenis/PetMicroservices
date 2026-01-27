using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Services.DTOs;

namespace WebApp.Controllers
{
    public class UserController(IUserService service) : Controller
    {
        private readonly IUserService _service = service;

        // GET: /Users
        public IActionResult Index(CancellationToken cancellationToken)
        {
            var users = _service.GetAllAsync(cancellationToken);
            return View(users);
        }

        // GET: /Users/Details/5
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var user = await _service.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET: /Users/Create
        public IActionResult Create() => View();

        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDto user, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(user);
            await _service.CreateAsync(user, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Edit/5
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var user = await _service.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UserDto user, CancellationToken cancellationToken)
        {
            if (id != user.Id) return BadRequest();
            if (!ModelState.IsValid) return View(user);

            await _service.UpdateAsync(user, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Delete/5
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await _service.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
        {
            var user = await _service.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();

            await _service.DeleteAsync(user.Id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}