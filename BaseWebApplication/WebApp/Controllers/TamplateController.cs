using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Template.Domain.Model;
using Template.Domain.Services;

namespace WebApp.Controllers
{
    public class TamplatesController(ITamplateService service, IUserService userService, ITagService tagService, ITopicService topicService) : Controller
    {
        private readonly ITamplateService _service = service;
        private readonly IUserService _userService = userService;
        private readonly ITagService _tagService = tagService;
        private readonly ITopicService _topicService = topicService;

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
        public IActionResult Create()
        {
            FillViewData();

            return View();
        }

        private void FillViewData()
        {
            ViewData["OwnerId"] = new SelectList(_userService.GetAllAsync(), "Id", "Name");
            ViewData["TopicId"] = new SelectList(_topicService.GetAllAsync(), "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_tagService.GetAllAsync(), "Id", "Name");
            ViewData["UsersAccess"] = new MultiSelectList(_userService.GetAllAsync(), "Id", "Name");
        }

        // POST: /Tamplates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tamplate tamplate, 
            Guid OwnerId, int TopicId, 
            List<int> Tags, List<Guid> UsersAccess, 
            CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                tamplate.Id = Guid.NewGuid();

                var user = await _userService.FindAsync(OwnerId, cancellationToken);
                if (user == null) 
                    return NotFound(user);

                tamplate.Owner = user;

                var topic = await _topicService.FindAsync(TopicId, cancellationToken);
                 if(topic == null) 
                    return NotFound(topic);

                tamplate.Topic = topic;

                // Todo
                //tamplate.Tags = await _tagService.Find(t => Tags.Contains(t.Id)).ToListAsync(cancellationToken);
                //tamplate.UsersAccess = await _context.Users.Where(u => UsersAccess.Contains(u.Id)).ToListAsync(cancellationToken);

                await _service.CreateAsync(tamplate, cancellationToken);

                return RedirectToAction(nameof(Index));
            }

            // repopulate dropdowns if validation fails
            FillViewData();

            return View(tamplate);
        }


        // GET: /Tamplates/Edit/5
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var tamplate = await _service.FindAsync(id, cancellationToken);
            if (tamplate == null) return NotFound();
            FillViewData();
            return View(tamplate);
        }

        // POST: /Tamplates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Tamplate tamplate, 
            Guid OwnerId, int TopicId, 
            List<int> Tags, List<Guid> UsersAccess,
     CancellationToken cancellationToken)
        {
            if (id != tamplate.Id) 
                return BadRequest();

            if (ModelState.IsValid)
            {
                var existing = await _service.FindAsync( id, cancellationToken);

                if (existing == null) return NotFound();

                // Update scalar properties
                existing.Title = tamplate.Title;
                existing.Description = tamplate.Description;

                // Update relationships
                var user = await _userService.FindAsync(OwnerId, cancellationToken);
                if (user == null)
                    return NotFound(user);

                existing.Owner = user;

                var topic = await _topicService.FindAsync(TopicId, cancellationToken);
                if (topic == null)
                    return NotFound(topic);

                existing.Topic = topic;

                //Todo
                //existing.Tags.Clear();
                //var newTags = await _context.Tags.Where(t => Tags.Contains(t.Id)).ToListAsync(cancellationToken);
                //foreach (var tag in newTags) existing.Tags.Add(tag);

                //existing.UsersAccess.Clear();
                //var newUsers = await _context.Users.Where(u => UsersAccess.Contains(u.Id)).ToListAsync(cancellationToken);
                //foreach (var user in newUsers) existing.UsersAccess.Add(user);

                await _service.UpdateAsync(existing, cancellationToken);
                return RedirectToAction(nameof(Index));
            }

            FillViewData();
            return View(tamplate);
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