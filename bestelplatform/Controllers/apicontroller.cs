using bestelplatform.Data.bestelplatform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using System.Dynamic;
using System.Net;

namespace bestelplatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class APIcontroller : ControllerBase
    {
        // Database ophalen.
        private readonly BestelplatformDbContext _bestelplatformContext;

        public APIcontroller(BestelplatformDbContext bestelplatformContext)
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
            return NotFound();
        }

        [HttpGet("history/ordered/drink/most")]
        public async Task<IActionResult> getMostOrderedDrinks()
        {
            var mostOrderedDrinks = await _bestelplatformContext.Bestellijnens
                                          .Select(line => new
                                          {
                                              orderLine = line,
                                              newestProductDetail = line.Product.Productdetails
                                                                    .OrderByDescending(pd => pd.Tijdstip)
                                                                    .FirstOrDefault()
                                          })
                                          .Where(x => x.newestProductDetail.Producttype.Contains("drank"))
                                          .GroupBy(x => x.newestProductDetail.Naam)
                                          .Select(group => new
                                          {
                                              productName = group.Key,
                                              amountSold = group.Sum(x => x.orderLine.Hoeveelheid)
                                          })
                                          .OrderByDescending(group => group.amountSold)
                                          .FirstOrDefaultAsync();

            if (mostOrderedDrinks == null)
            {
                return NotFound(new { Bericht = "Geen drankbestellingen gevonden." });
            }

            return Ok(mostOrderedDrinks);
        }

        [HttpGet("history/ordered/drink/least")]
        public async Task<IActionResult> getLeastOrderedDrinks()
        {
            var leastOrderedDrinks = await _bestelplatformContext.Bestellijnens
                                          .Select(line => new
                                          {
                                              orderLine = line,
                                              newestProductDetail = line.Product.Productdetails
                                                                    .OrderByDescending(pd => pd.Tijdstip)
                                                                    .FirstOrDefault()
                                          })
                                          .Where(x => x.newestProductDetail.Producttype.Contains("drank"))
                                          .GroupBy(x => x.newestProductDetail.Naam)
                                          .Select(group => new
                                          {
                                              productName = group.Key,
                                              amountSold = group.Sum(x => x.orderLine.Hoeveelheid)
                                          })
                                          .OrderBy(group => group.amountSold)
                                          .FirstOrDefaultAsync();

            if (leastOrderedDrinks == null)
            {
                return NotFound(new { Bericht = "Geen drankbestellingen gevonden." });
            }

            return Ok(leastOrderedDrinks);
        }

        [HttpGet("history/ordered/snack/most")]
        public async Task<IActionResult> getLeastOrderedSnack()
        {
            var getMostOrderedSnack = await _bestelplatformContext.Bestellijnens
                                          .Select(line => new
                                          {
                                              orderLine = line,
                                              newestProductDetail = line.Product.Productdetails
                                                                    .OrderByDescending(pd => pd.Tijdstip)
                                                                    .FirstOrDefault()
                                          })
                                          .Where(x => x.newestProductDetail.Producttype.Contains("versnapering"))
                                          .GroupBy(x => x.newestProductDetail.Naam)
                                          .Select(group => new
                                          {
                                              productName = group.Key,
                                              amountSold = group.Sum(x => x.orderLine.Hoeveelheid)
                                          })
                                          .OrderByDescending(group => group.amountSold)
                                          .FirstOrDefaultAsync();

            if (getMostOrderedSnack == null)
            {
                return NotFound(new { Bericht = "Geen versnaperingbestellingen gevonden." });
            }

            return Ok(getMostOrderedSnack);
        }

        [HttpGet("history/ordered/snack/least")]
        public async Task<IActionResult> getMostOrderedSnack()
        {
            var getLeastOrderedSnack = await _bestelplatformContext.Bestellijnens
                                          .Select(line => new
                                          {
                                              orderLine = line,
                                              newestProductDetail = line.Product.Productdetails
                                                                    .OrderByDescending(pd => pd.Tijdstip)
                                                                    .FirstOrDefault()
                                          })
                                          .Where(x => x.newestProductDetail.Producttype.Contains("versnapering"))
                                          .GroupBy(x => x.newestProductDetail.Naam)
                                          .Select(group => new
                                          {
                                              productName = group.Key,
                                              amountSold = group.Sum(x => x.orderLine.Hoeveelheid)
                                          })
                                          .OrderBy(group => group.amountSold)
                                          .FirstOrDefaultAsync();

            if (getLeastOrderedSnack == null)
            {
                return NotFound(new { Bericht = "Geen versnaperingbestellingen gevonden." });
            }

            return Ok(getLeastOrderedSnack);
        }

        [HttpGet("history/table/ordered/drinks/most")]
        public async Task<IActionResult> getMostOrderedDrinksTable()
        {
            var getMostOrderedDrinksTable = await _bestelplatformContext.Bestellijnens
                                                           .Select(line => new
                                                           {
                                                               orderLine = line,
                                                               assignedTable = line.Bestelling.Gebruiker.Tafeltoewijzingens
                                                                               .Where(x => x.TijdstipToegewezen < line.Bestelling.TijdstipBesteld)
                                                                               .OrderByDescending(tt => tt.TijdstipToegewezen)
                                                                               .FirstOrDefault(),
                                                               newestProductDetail = line.Product.Productdetails
                                                                                      .OrderByDescending(pd => pd.Tijdstip)
                                                                                      .FirstOrDefault()
                                                           })
                                                           .Where(x => x.newestProductDetail.Producttype.Contains("drank"))
                                                           .GroupBy(x => x.assignedTable.Tafel.Nummer)
                                                           .Select(group => new
                                                           {
                                                               tableNumber = group.Key,
                                                               totalPrice = group.Sum(x => x.orderLine.Hoeveelheid * x.newestProductDetail.Prijs)
                                                           })
                                                           .OrderByDescending(result => result.totalPrice)
                                                           .FirstOrDefaultAsync();
            if (getMostOrderedDrinksTable == null)
            {
                return NotFound(new { Bericht = "Nog geen tafels met drankbestellingen" });
            }

            var formatResult = new
            {
                tableNumber = getMostOrderedDrinksTable.tableNumber,
                totalPrice = $"€ {getMostOrderedDrinksTable.totalPrice.ToString("F2")}"
            };

            return Ok(formatResult);
        }

        [HttpGet("history/table/ordered/snacks/most")]
        public async Task<IActionResult> getMostOrderedSnackTable()
        {
            var getMostOrderedSnacksTable = await _bestelplatformContext.Bestellijnens
                                                           .Select(line => new
                                                           {
                                                               orderLine = line,
                                                               assignedTable = line.Bestelling.Gebruiker.Tafeltoewijzingens
                                                                               .Where(x => x.TijdstipToegewezen < line.Bestelling.TijdstipBesteld)
                                                                               .OrderByDescending(tt => tt.TijdstipToegewezen)
                                                                               .FirstOrDefault(),
                                                               newestProductDetail = line.Product.Productdetails
                                                                                      .OrderByDescending(pd => pd.Tijdstip)
                                                                                      .FirstOrDefault()
                                                           })
                                                           .Where(x => x.newestProductDetail.Producttype.Contains("versnapering"))
                                                           .GroupBy(x => x.assignedTable.Tafel.Nummer)
                                                           .Select(group => new
                                                           {
                                                               tableNumber = group.Key,
                                                               totalPrice = group.Sum(x => x.orderLine.Hoeveelheid * x.newestProductDetail.Prijs)
                                                           })
                                                           .OrderByDescending(result => result.totalPrice)
                                                           .FirstOrDefaultAsync();
            if (getMostOrderedSnacksTable == null)
            {
                return NotFound(new { Bericht = "Nog geen tafels met drankbestellingen" });
            }

            var formatResult = new
            {
                tableNumber = getMostOrderedSnacksTable.tableNumber,
                totalPrice = $"€ {getMostOrderedSnacksTable.totalPrice.ToString("F2")}"
            };

            return Ok(formatResult);
        }

    }
}
