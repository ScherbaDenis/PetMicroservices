using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Services;
using WebApp.Services.DTOs;

namespace WebApp.Controllers
{
    public class TemplateController(ITemplateService service, IUserService userService, ITopicService topicService) : Controller
    {
        private readonly ITemplateService _service = service;
        private readonly IUserService _userService = userService;
        private readonly ITopicService _topicService = topicService;

        // GET: /Templates
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);
            return View(items);
        }

        // GET: /Templates/Details/{id}
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Templates/Create
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            await PopulateDropdownsAsync(cancellationToken);
            return View();
        }

        // POST: /Templates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TemplateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(cancellationToken);
                return View(dto);
            }
            await _service.CreateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Templates/Edit/{id}
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();
            await PopulateDropdownsAsync(cancellationToken);
            return View(item);
        }

        // POST: /Templates/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TemplateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(cancellationToken);
                return View(dto);
            }

            await _service.UpdateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Templates/Delete/{id}
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Templates/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();

            await _service.DeleteAsync(item.Id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Populates ViewBag with dropdown options for Owner and Topic
        /// </summary>
        private async Task PopulateDropdownsAsync(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            var topics = await _topicService.GetAllAsync(cancellationToken);

            ViewBag.OwnerId = new SelectList(users, "Id", "Name");
            ViewBag.TopicId = new SelectList(topics, "Id", "Name");
        }
    }
}