using Microsoft.AspNetCore.Mvc;

namespace bestelplatform.Data.bestelplatform
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
