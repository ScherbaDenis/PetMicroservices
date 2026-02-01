using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Services.DTOs;

namespace WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly ITemplateService _templateService;

        public UserController(IUserService service, ITemplateService templateService)
        {
            _service = service;
            _templateService = templateService;
        }

        // GET: /Users
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _service.GetAllAsync(cancellationToken);
            return View(users);
        }

        // GET: /Users/Details/5
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var user = await _service.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return View(user);
        }

        /// <summary>
        /// API endpoint to get templates for a specific user as JSON
        /// Example: GET /User/GetTemplates/11111111-1111-1111-1111-111111111111
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>JSON array of templates</returns>
        [HttpGet]
        public async Task<IActionResult> GetTemplates(Guid id, CancellationToken cancellationToken)
        {
            var user = await _service.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound(new { error = "User not found" });

            // Example usage of GetByUserIdAsync method
            var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
            
            return Json(new 
            { 
                userId = user.Id, 
                userName = user.Name, 
                templates = templates,
                count = templates.Count()
            });
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