using Microsoft.AspNetCore.Mvc;

namespace bestelplatform.Controllers
{
    [Route("[controller]")]
    public class bestelController : Controller
    {
        public IActionResult bestelPagina()
        {
            return View();
        }
    }
}
