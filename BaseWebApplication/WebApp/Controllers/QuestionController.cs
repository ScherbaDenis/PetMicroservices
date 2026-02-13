using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Services.DTOs;

namespace WebApp.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ILogger<QuestionController> _logger;
        private readonly IQuestionService _questionService;

        public QuestionController(ILogger<QuestionController> logger, IQuestionService questionService)
        {
            _logger = logger;
            _questionService = questionService;
        }

        // GET: /Question
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var questions = await _questionService.GetAllAsync(cancellationToken);
                return View(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching questions");
                return View("Error");
            }
        }

        // GET: /Question/Details/5
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id, cancellationToken);
                if (question == null)
                {
                    return NotFound();
                }
                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching question {Id}", id);
                return View("Error");
            }
        }

        // GET: /Question/Create
        public IActionResult Create(string? questionType = "SingleLineString")
        {
            ViewBag.QuestionType = questionType;
            return View();
        }

        // POST: /Question/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionDto questionDto, CancellationToken cancellationToken)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _questionService.CreateAsync(questionDto, cancellationToken);
                    return RedirectToAction(nameof(Index));
                }
                return View(questionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                ModelState.AddModelError("", "An error occurred while creating the question.");
                return View(questionDto);
            }
        }

        // GET: /Question/Edit/5
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id, cancellationToken);
                if (question == null)
                {
                    return NotFound();
                }
                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching question {Id} for edit", id);
                return View("Error");
            }
        }

        // POST: /Question/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, QuestionDto questionDto, CancellationToken cancellationToken)
        {
            if (id != questionDto.Id)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _questionService.UpdateAsync(questionDto, cancellationToken);
                    return RedirectToAction(nameof(Index));
                }
                return View(questionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the question.");
                return View(questionDto);
            }
        }

        // GET: /Question/Delete/5
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id, cancellationToken);
                if (question == null)
                {
                    return NotFound();
                }
                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching question {Id} for delete", id);
                return View("Error");
            }
        }

        // POST: /Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _questionService.DeleteAsync(id, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question {Id}", id);
                ModelState.AddModelError("", "An error occurred while deleting the question.");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
