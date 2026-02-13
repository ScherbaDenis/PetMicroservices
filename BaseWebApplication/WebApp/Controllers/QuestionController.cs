using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ILogger<QuestionController> logger)
        {
            _logger = logger;
        }

        // GET: /Question
        public IActionResult Index()
        {
            return View();
        }
    }
}
