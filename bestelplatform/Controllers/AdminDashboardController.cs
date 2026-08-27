using bestelplatform.Data.bestelplatform;
using bestelplatform.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mysqlx;
using QRCoder;

namespace bestelplatform.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdmindashboardController : Controller
    {
        private readonly BestelplatformDbContext _bestelplatformContext;
        public AdmindashboardController(BestelplatformDbContext bestelplatformContext, IConfiguration configuration)
        {
            _bestelplatformContext = bestelplatformContext;
        }
        [Route("")]
        [Route("gebruiksbeheer")]
        public IActionResult GebruiksBeheer()
        {
            return View("GebruikersBeheer");
        }

        [HttpGet("add/user")]
        public async Task<IActionResult> AddUserAndGenerateQRCode()
        {
            string uniqueCode = Guid.NewGuid().ToString();
            string url = $"{Request.Scheme}://{Request.Host}/registratie?uniekecode={uniqueCode}";

            var urlPayload = new PayloadGenerator.Url(url);
            using var qrCodeData = QRCodeGenerator.GenerateQrCode(urlPayload, QRCodeGenerator.ECCLevel.H);
            using var pngRender = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = pngRender.GetGraphic(6);
            string base64Image = "data:image/png;base64," + Convert.ToBase64String(qrCodeImage);

            var gebruiker = new Gebruiker
            {
                UniekeCode = uniqueCode
            };
            await _bestelplatformContext.AddAsync(gebruiker);

            var medewerker = new Medewerker
            {
                Gebruiker = gebruiker
            };
            await _bestelplatformContext.AddAsync(medewerker);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok(new { 
                ImageSrc = base64Image,
                Url = url                 
            });
        }

        [HttpGet("load/users")]
        public async Task<IActionResult> LoadUsers()
        {
            var users = await _bestelplatformContext.Gebruikers
            .Select(user => new
            {
                id = user.Id,
                naam = user.Naam,
                uniekeCode = user.UniekeCode,
                geactiveerd = user.Geactiveerd,
                roles = user.Rols.Select(r => new { naam = r.Naam }).ToList()
            })
            .OrderByDescending(user => user.geactiveerd)
            .ToListAsync();
            return Ok(users);
        }

        [HttpDelete("remove/user")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            var gebruiker = await _bestelplatformContext.Gebruikers
                                  .FindAsync(id);
            _bestelplatformContext.Gebruikers.Remove(gebruiker);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }
        [HttpGet("get/roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _bestelplatformContext.Rollens
                        .ToListAsync();
            return Ok(roles);
        }

        public class AddRolePostBody()
        {
            public int UserId { get; set; }
            public string NewRole { get; set; }
        }

        [HttpPost("add/user/role")]
        public async Task<IActionResult> AddUserRole([FromBody] AddRolePostBody rolePostBody)
        {
            var newRole = await _bestelplatformContext.Rollens
                                    .FirstOrDefaultAsync(row => row.Naam == rolePostBody.NewRole);
            var gebruiker = await _bestelplatformContext.Gebruikers
                                  .Include(table => table.Rols)
                                  .FirstOrDefaultAsync(row => row.Id == rolePostBody.UserId);

            foreach (var role in gebruiker.Rols)
            {
                if (role.Naam == rolePostBody.NewRole)
                {
                    return Conflict();
                }
            }
            gebruiker.Rols.Add(newRole);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete/user/role")]
        public async Task<IActionResult> DeleteUserRole(int userId, string roleName)
        {
            var toBeDeletedRole = await _bestelplatformContext.Rollens
                                    .FirstOrDefaultAsync(row => row.Naam == roleName);
            var gebruiker = await _bestelplatformContext.Gebruikers
                                  .Include(table => table.Rols)
                                  .FirstOrDefaultAsync(row => row.Id == userId);

            gebruiker.Rols.Remove(toBeDeletedRole);
            await _bestelplatformContext.SaveChangesAsync();

            return Ok();
        }
    }
}
