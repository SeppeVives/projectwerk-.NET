using bestelplatform.Data.bestelplatform;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bestelplatform.Controllers
{
    [Route("[Controller]")]
    public class RegistratieController : Controller
    {
        private readonly BestelplatformDbContext _bestelplatformContext;
        public RegistratieController(BestelplatformDbContext bestelplatformContext, IConfiguration configuration)
        {
            _bestelplatformContext = bestelplatformContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> ShowRegistrationPage(string? uniekecode)
        {
            var uniqueCode = uniekecode;
            var cookieToken = Request.Cookies["UserCookie"];
            if (!string.IsNullOrEmpty(cookieToken))
            {
                Response.Cookies.Delete("UserCookie");
            }
            if (!String.IsNullOrEmpty(uniqueCode))
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(24),
                    HttpOnly = true,
                    Secure = false, // Zet dit later op true wannneer je de app in the cloud host.
                    IsEssential = true
                };
                Response.Cookies.Append("UserCookie", uniqueCode, cookieOptions);
            }

            return View("RegistratiePagina");
        }

        [HttpPost("submit")]
        public async Task<IActionResult> RegistrationSubmit(string userName, string password)
        {
            var passwordHasher = new PasswordHasher<Gebruiker>();
            var cookieToken = Request.Cookies["UserCookie"];

            if (!String.IsNullOrEmpty(cookieToken))
            {
                var currentUser = await _bestelplatformContext.Gebruikers
                                        .FirstOrDefaultAsync(row => row.UniekeCode == cookieToken);
                var users = await _bestelplatformContext.Gebruikers
                                  .ToListAsync();
                bool checkIfUserExists = await _bestelplatformContext.Gebruikers
                                               .AnyAsync(row => row.Naam.ToLower() == userName.ToLower());

                if (!checkIfUserExists)
                {
                    if (userName.Contains(" "))
                    {
                        ViewBag.EnteredUsername = userName;
                        ModelState.AddModelError("userName", "Gebruikersnaam mag geen spaties bevatten");
                        return View("RegistratiePagina");
                    }
                    var hashedPassword = passwordHasher.HashPassword(currentUser, password);

                    currentUser.Naam = userName;
                    currentUser.WachtwoordHash = hashedPassword;
                    currentUser.Geactiveerd = true;
                    _bestelplatformContext.Update(currentUser);
                    await _bestelplatformContext.SaveChangesAsync();
                    ModelState.AddModelError("registrationFinished", "Registratie gelukt.");
                    return View("RegistratiePagina");
                }
                else
                {
                    ModelState.AddModelError("userName", "Deze gebruikersnaam is al in gebruik. Kies een andere gebruikersnaam.");
                    return View("RegistratiePagina");
                }

            }
            else
            {
                return View("RegistratiePagina");
            }
        }
    }
}
