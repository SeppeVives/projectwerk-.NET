using bestelplatform.Data.bestelplatform;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bestelplatform.Controllers
{
    [Route("[Controller]")]
    public class LoginController : Controller
    {
        private readonly BestelplatformDbContext _bestelplatformContext;
        public LoginController(BestelplatformDbContext bestelplatformContext, IConfiguration configuration)
        {
            _bestelplatformContext = bestelplatformContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> ShowLoginPage()
        {
            var cookieToken = Request.Cookies["UserCookie"];
            if (!string.IsNullOrEmpty(cookieToken))
            {
                Response.Cookies.Delete("UserCookie");
            }
            return View("LoginPagina");
        }

        [HttpPost("submit")]
        public async Task<IActionResult> LoginSubmit(string userName, string password)
        {
            var passwordHasher = new PasswordHasher<Gebruiker>();
            var cookieToken = Request.Cookies["UserCookie"];
            var currentUser = await _bestelplatformContext.Gebruikers
                                    .Include(table => table.Rols)
                                    .FirstOrDefaultAsync(row => row.Naam == userName);

            if (currentUser == null)
            {
                ModelState.AddModelError("userName", "De gebruikersnaam bestaat niet.");
                return View("LoginPagina");
            }
            else
            {
                var result = passwordHasher.VerifyHashedPassword(currentUser, currentUser.WachtwoordHash, password);

                if(result == PasswordVerificationResult.Success)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddHours(24),
                        HttpOnly = true,
                        Secure = false, // Zet dit later op true wannneer je de app in the cloud host.
                        IsEssential = true
                    };
                    Response.Cookies.Append("UserCookie", currentUser.UniekeCode, cookieOptions);
                    return Ok();
                }
                else
                {
                    ViewBag.EnteredUsername = userName;
                    ModelState.AddModelError("passWord", "Wachtwoord klopt niet.");
                    return View("LoginPagina");
                }
            }
        }
    }
}
