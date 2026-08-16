using bestelplatform.Data.bestelplatform; // database
using bestelplatform.Models.Enums;
using bestelplatform.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mollie.Api.Client;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;
using Mollie.Api.Models.Profile.Response;
using Mysqlx.Resultset;
using Org.BouncyCastle.Asn1.Cms;
using System.Diagnostics;
using System.Net;

namespace bestelplatform.Controllers
{
    [Route("[controller]")]
    public class bestelController : Controller
    {
        private readonly BestelplatformContext _bestelplatformContext;
        private readonly string _mollieApiKey;

        public bestelController(BestelplatformContext bestelplatformContext, IConfiguration configuration)
        {
            _bestelplatformContext = bestelplatformContext;
            _mollieApiKey = configuration["Mollie:ApiKey"]!;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int tafelnummer = 0)
        {
            var model = new bestelPaginaViewModel();
            // Nieuwe gebruiker toevoegen en cookie aanmaken.
            string? cookieToken = Request.Cookies["UserCookie"];
            string? uniqueCode = null;
            // tabelobjecten initialisatie
            Bezoeker? newVisitor = null;
            Tafeltoewijzingen? newTableAssignment = null;
            if (!String.IsNullOrEmpty(cookieToken))
            {
                var oldUser = await _bestelplatformContext.Gebruikers
                                    .Where(row => row.UniekeCode == cookieToken)
                                    .FirstOrDefaultAsync();
                if (oldUser != null)
                {
                    uniqueCode = oldUser.UniekeCode;
                }
            }
            if (string.IsNullOrEmpty(uniqueCode))
            {
                uniqueCode = Guid.NewGuid().ToString();
                string userName = $"bezoeker#{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
                Gebruiker currentUser = new Gebruiker
                {
                    UniekeCode = uniqueCode,
                    Naam = userName
                };
                newVisitor = new Bezoeker
                {
                    Gebruiker = currentUser
                };

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(24),
                    HttpOnly = true,
                    Secure = false, // Zet dit later op true wannneer je de app in the cloud host.
                    IsEssential = true
                };
                Response.Cookies.Append("UserCookie", uniqueCode, cookieOptions);
                // Tabellen voor tafels invullen. Ook een nieuwe rij aanmaken bij tafelwisseling.
                var currentTable = await _bestelplatformContext.Tafels
                                  .Where(row => row.Nummer == tafelnummer)
                                  .FirstOrDefaultAsync();
                newTableAssignment = new Tafeltoewijzingen
                {
                    Gebruiker = newVisitor,
                    Tafel = currentTable,
                    TijdstipToegewezen = DateTime.Now
                };
                _bestelplatformContext.Add(newTableAssignment);
                await _bestelplatformContext.SaveChangesAsync();
            }
            // Model vullen met data uit productdetails.
            model.TableNumber = tafelnummer;
            var productDetailsData = await _bestelplatformContext.Productdetails
                .Select(tabel => new ProductDetails
                {
                    ProductName = tabel.Naam,
                    ProductID = tabel.ProductId,
                    ProductPrice = tabel.Prijs,
                    ProductType = tabel.Producttype,
                })
                .ToListAsync();
            model.ProductDetails = productDetailsData;
            return View("bestelpagina", model);
        }

        [HttpPost("overzicht")]
        public async Task<IActionResult> overzicht(List<OrderInputProperties> bestelInputs, int tafelNummer)
        {
            var model = new overzichtPaginaModel();
            model.TableNumber = tafelNummer;
            var productDetails = await _bestelplatformContext.Productdetails
                .ToListAsync();
            var orderedProductProperties = new List<OrderedProductProperties>();
            float totaalPrijs = 0;
            // De verstuurde ID's van de form filteren (groter dan 0) met LINQ.
            foreach (var bestelInput in bestelInputs)
            {
                if (bestelInput.Amount > 0)
                {
                    var unitPrice = productDetails
                        .Where(row => row.ProductId == bestelInput.InputID)
                        .Select(row => row.Prijs)
                        .FirstOrDefault();
                    var subtotalPrice = bestelInput.Amount * unitPrice;
                    orderedProductProperties.Add(new OrderedProductProperties
                    {
                        UnitPrice = unitPrice,
                        SubtotalPrice = subtotalPrice,
                        Amount = bestelInput.Amount,
                        ProductName = bestelInput.ProductName,
                        ProductID = bestelInput.InputID
                    });
                    totaalPrijs += subtotalPrice;
                }
            }
            model.OrderInputProperties = bestelInputs;
            model.OrderedProductProperties = orderedProductProperties;
            model.TotalPrice = totaalPrijs;
            return View("overzichtPagina", model);
        }
        [HttpPost("betaling")]
        public async Task<IActionResult> betaling(int tableNumber, float totalPrice, List<OrderedProductProperties> orderedProductProperties)
        {
            // Huidige anonieme gebruiker ophalen met cookie.
            string? cookieToken = Request.Cookies["UserCookie"];
            // Record aanmaken voor bestellingen.
            var activeUser = await _bestelplatformContext.Bezoekers
                                      .Where(row => row.Gebruiker.UniekeCode == cookieToken)
                                      .FirstOrDefaultAsync();
            var newOrder = new Bestellingen
            {
                Gebruiker = activeUser,
                TijdstipBesteld = DateTime.Now,
                Status = "niet betaald"
            };
            _bestelplatformContext.Add(newOrder);
            await _bestelplatformContext.SaveChangesAsync();
            // Record aanmaken voor bestellijnen.
            foreach (var orderedProduct in orderedProductProperties)
            {
                var newOrderLine = new Bestellijnen
                {
                    BestellingId = newOrder.Id,
                    ProductId = orderedProduct.ProductID,
                    Hoeveelheid = orderedProduct.Amount
                };
                _bestelplatformContext.Add(newOrderLine);
            }
            await _bestelplatformContext.SaveChangesAsync();
            //Mollie testbetaling laden.
            int orderID = newOrder.Id;
            using var paymentClient = new PaymentClient(_mollieApiKey);
            var paymentRequest = new PaymentRequest
            {
                Amount = new Amount(Currency.EUR, totalPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)),
                Description = $"Test ORW Wielewaal - Tafel {tableNumber}",
                RedirectUrl = $"http://localhost:5005/bestel/status",
            };
            PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);
            var updateRequest = new PaymentUpdateRequest
            {
                RedirectUrl = $"http://localhost:5005/bestel/status?paymentid={paymentResponse.Id}"
            };
            await paymentClient.UpdatePaymentAsync(paymentResponse.Id, updateRequest);
            return Redirect(paymentResponse.Links.Checkout.Href);
        }
        [HttpGet("status")]
        public async Task<IActionResult> status(string paymentid)
        {  
            // Na betaling de status van de bestelling aanpassen.
            string? cookieToken = Request.Cookies["UserCookie"];
            var bestelling = await _bestelplatformContext.Bestellingens
                                   .Where(row => row.Gebruiker.Gebruiker.UniekeCode == cookieToken)
                                   .OrderByDescending(row => row.TijdstipBesteld)
                                   .FirstOrDefaultAsync();

            using var paymentClient = new PaymentClient(_mollieApiKey);
            PaymentResponse payment = await paymentClient.GetPaymentAsync(paymentid);
            if (payment.Status == PaymentStatus.Paid)
            {
                bestelling.Status = "besteld";
                await _bestelplatformContext.SaveChangesAsync();
            }
            return View("statusPagina");
            // De bestelling meegeven aan een model.
        }
    }
    public class ProductDetails
    {
        public string ProductName { get; set; } = string.Empty;
        public int ProductID { get; set; }
        public float ProductPrice { get; set; }
        public ProductEnum ProductType { get; set; }

    }
    public class OrderInputProperties
    {
        public int InputID { get; set; }
        public int Amount { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
    public class OrderedProductProperties
    {
        public float UnitPrice { get; set; }
        public float SubtotalPrice { get; set; }
        public int Amount { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ProductID { get; set; }
    }
}
