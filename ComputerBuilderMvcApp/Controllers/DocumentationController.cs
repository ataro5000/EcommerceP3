using Microsoft.AspNetCore.Mvc;

namespace ComputerBuilderMvcApp.Controllers
{
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}