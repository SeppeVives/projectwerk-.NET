using Microsoft.AspNetCore.Mvc;

namespace bestelplatform.Controllers
{
    [ApiController] // Geeft aan dat dit een API is (stuurt JSON)
    [Route("[controller]")] // De URL wordt dan: api/Statistiek
    public class APIcontroller : ControllerBase // ControllerBase is voor API's zonder Views
    {
        // Hier komen dadelijk je methodes voor de statistieken

        [HttpGet("test")]
        public IActionResult GetTest()
        {
            return Ok(new { bericht = "De API werkt!" });
        }
    }
}
