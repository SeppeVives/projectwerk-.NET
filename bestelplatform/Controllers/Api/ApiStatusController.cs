using bestelplatform.Data.bestelplatform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using System.Dynamic;
using System.Net;

namespace bestelplatform.Controllers.Api
{
    [ApiController]
    [Route("api")]
    public class APIStatuscontroller : ControllerBase
    {
        // Database ophalen.
        private readonly BestelplatformDbContext _bestelplatformContext;

        public APIStatuscontroller(BestelplatformDbContext bestelplatformContext)
        {
            _bestelplatformContext = bestelplatformContext;
        }

        [HttpGet("visitor/orders/statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var cookieToken = Request.Cookies["UserCookie"];
            if (cookieToken != null)
            {
                var visitorOrderStatus = await _bestelplatformContext.Bestellingens
                                       .Where(row => row.Gebruiker.Gebruiker.UniekeCode == cookieToken)
                                       .OrderByDescending(row => row.TijdstipBesteld)
                                       .Select(row => row.Status)
                                       .ToListAsync();
                return Ok(visitorOrderStatus);
            }
            return Unauthorized();
        }
    }
}
