using bestelplatform.Data.bestelplatform; // database
using bestelplatform.Models.Enums;
using bestelplatform.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mollie.Api.Client;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;
using Mollie.Api.Models.Profile.Response;
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
            if (!String.IsNullOrEmpty(cookieToken))
            {
                uniqueCode = await _bestelplatformContext.Gebruikers
                                    .Where(row => row.UniekeCode == cookieToken)
                                    .Select(row => row.UniekeCode)
                                    .FirstOrDefaultAsync();
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
                var newVisitor = new Bezoeker
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

                // Tabellen voor tafels invullen.
                var newTable = new Tafel
                {
                    Nummer = tafelnummer
                };
                var newTableAssignment = new Tafeltoewijzingen
                {
                    Gebruiker = newVisitor,
                    Tafel = newTable,
                    TijdstipToegewezen = DateTime.Now
                };
                _bestelplatformContext.Add(newTableAssignment);
                await _bestelplatformContext.SaveChangesAsync();
                Response.Cookies.Append("UserCookie", uniqueCode, cookieOptions);
            }

            // Model vullen met data uit productdetails.
            model.TableNumber = tafelnummer;
            var productDetailsData = await _bestelplatformContext.Productdetails
                .Select(tabel => new ProductDetails
                {
                    ProductName = tabel.Naam,
                    ProductID = tabel.ProductId,
                    ProductPrice = tabel.Prijs,
                    ProductType = tabel.Producttype
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
                        ProductName = bestelInput.ProductName
                    });
                    totaalPrijs += subtotalPrice;
                }
            }
            model.BestelInputProperties = bestelInputs;
            model.BesteldProductProperties = orderedProductProperties;
            model.TotalPrice = totaalPrijs;
            return View("overzichtPagina", model);
        }
        [HttpPost("betaling")]
        public async Task<IActionResult> betaling(int tableNumber, float totalPrice)
        {
            using var paymentClient = new PaymentClient(_mollieApiKey);
            var paymentRequest = new PaymentRequest
            {
                Amount = new Amount(Currency.EUR, totalPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)),
                Description = $"Test ORW Wielewaal - Tafel {tableNumber}",
                RedirectUrl = "http://localhost:5005/bestel/status"
            };
            PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);
            return Redirect(paymentResponse.Links.Checkout.Href);
        }
        [HttpGet("status")]
        public async Task<IActionResult> status()
        {
            return View("statusPagina");
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
    }
}
