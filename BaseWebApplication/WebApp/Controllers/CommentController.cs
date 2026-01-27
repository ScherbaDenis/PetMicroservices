using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Services.DTOs;

namespace WebApp.Controllers
{
    public class CommentController(ICommentService service, ITemplateService templateService) : Controller
    {
        private readonly ICommentService _service = service;
        private readonly ITemplateService _templateService = templateService;

        private async Task PopulateTemplatesViewBag(CancellationToken cancellationToken)
        {
            var templates = await _templateService.GetAllAsync(cancellationToken);
            ViewBag.Templates = templates.Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Title
            }).ToList();
        }

        // GET: /Comment
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);
            return View(items);
        }

        // GET: /Comment/Details/{id}
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Comment/Create
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            await PopulateTemplatesViewBag(cancellationToken);
            return View();
        }

        // POST: /Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateTemplatesViewBag(cancellationToken);
                return View(dto);
            }
            await _service.CreateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Comment/Edit/{id}
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();
            
            await PopulateTemplatesViewBag(cancellationToken);
            return View(item);
        }

        // POST: /Comment/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CommentDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateTemplatesViewBag(cancellationToken);
                return View(dto);
            }

            await _service.UpdateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Comment/Delete/{id}
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Comment/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();

            await _service.DeleteAsync(item.Id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
