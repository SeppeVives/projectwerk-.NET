using bestelplatform.Data.bestelplatform; // database
using bestelplatform.Models.Enums;
using bestelplatform.Models.Views;
using bestelplatform.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mollie.Api.Client;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;
using Mollie.Api.Models.Profile.Response;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace bestelplatform.Controllers
{
    [Route("[controller]")]
    public class bestelController : Controller
    {
        private readonly BestelplatformDbContext _bestelplatformContext;
        private readonly string _mollieApiKey;

        public bestelController(BestelplatformDbContext bestelplatformContext, IConfiguration configuration)
        {
            _bestelplatformContext = bestelplatformContext;
            _mollieApiKey = configuration["Mollie:ApiKey"]!;
        }
        private async Task<bool> CheckIfUserExists(Bezoeker currentVisitor)
        {
            if (currentVisitor == null)
            {
                Response.Cookies.Delete("UserCookie");
                return false;
            }
            return true;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int tafelnummer = 0)
        {
            var model = new bestelPaginaViewModel();
            Tafeltoewijzingen? newTableAssignment = null;
            string? cookieToken = Request.Cookies["UserCookie"];
            string? uniqueCode = null;
            Bezoeker? newVisitor = null;
            if (!String.IsNullOrEmpty(cookieToken))
            {
                var currentVisitor = await _bestelplatformContext.Bezoekers
                                             .Where(row => row.Gebruiker.UniekeCode == cookieToken)
                                             .FirstOrDefaultAsync();
                if (!await CheckIfUserExists(currentVisitor))
                {
                    return RedirectToAction("Index", new { tafelnummer = tafelnummer });
                }
                var currentTable = await _bestelplatformContext.Tafeltoewijzingens
                                    .Where(row => row.Gebruiker.Gebruiker.UniekeCode == cookieToken)
                                    .OrderByDescending(row => row.TijdstipToegewezen)
                                    .Select(row => row.Tafel.Nummer)
                                    .FirstOrDefaultAsync();
                // Als de gebruiker met cookie wilt veranderen van tafel.
                if (tafelnummer == 0)
                {
                    tafelnummer = currentTable;
                }
                else
                {
                    if (currentTable != tafelnummer)
                    {
                        // Controleren of het een bestaande tafel is.
                        var newTable = await _bestelplatformContext.Tafels
                                             .FirstOrDefaultAsync(row => row.Nummer == tafelnummer);

                        if (newTable != null)
                        {
                            newTableAssignment = new Tafeltoewijzingen
                            {
                                GebruikerId = currentVisitor.GebruikerId,
                                Tafel = newTable,
                                TijdstipToegewezen = DateTime.Now
                            };
                            _bestelplatformContext.Add(newTableAssignment);
                            await _bestelplatformContext.SaveChangesAsync();
                        }
                        else
                        {
                            return View("FoutPagina");
                        }
                    }
                }
            }
            else
            {
                // Allereerst een controle of er een QR-code gescand werd.
                var currentTable = await _bestelplatformContext.Tafels
                                      .Where(row => row.Nummer == tafelnummer)
                                      .FirstOrDefaultAsync();
                if (currentTable == null)
                {
                    return View("FoutPagina");
                }
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
                _bestelplatformContext.Bezoekers.Add(newVisitor);
                await _bestelplatformContext.SaveChangesAsync();

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(24),
                    HttpOnly = true,
                    Secure = false, // Zet dit later op true wannneer je de app in the cloud host.
                    IsEssential = true
                };
                Response.Cookies.Append("UserCookie", uniqueCode, cookieOptions);
                // Tabellen voor tafels invullen. Ook een nieuwe rij aanmaken bij tafelwisseling.
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
                .Select(tabel => new ProductdetailsDto
                {
                    ProductName = tabel.Naam,
                    ProductID = tabel.ProductId,
                    ProductPrice = tabel.Prijs,
                    ProductType = tabel.Producttype
                })
                .ToListAsync();
            model.ProductDetails = productDetailsData;
            return View("Bestelpagina", model);
        }

        // Wanneer de bezoeker zomaar naar /bestel/overzicht zou surfen zonder httpPOST.
        [HttpGet("overzicht")]
        public async Task<IActionResult> Overzicht()
        {
            string? cookieToken = Request.Cookies["UserCookie"];
            if (cookieToken == null)
            {
                return View("FoutPagina");
            }
            else
            {
                var tableNumber = await _bestelplatformContext.Tafeltoewijzingens
                                    .Where(row => row.Gebruiker.Gebruiker.UniekeCode == cookieToken)
                                    .Select(row => row.Tafel.Nummer)
                                    .FirstOrDefaultAsync();
                return RedirectToAction("index", new { tafelnummer = tableNumber });
            }
        }

        [HttpPost("overzicht")]
        public async Task<IActionResult> Overzicht(List<OrderInputProperties> bestelInputs)
        {
            string? cookieToken = Request.Cookies["UserCookie"];
            if (cookieToken == null)
            {
                return View("FoutPagina");
            }
            var tableNumber = await _bestelplatformContext.Tafeltoewijzingens
                                      .Where(row => row.Gebruiker.Gebruiker.UniekeCode == cookieToken)
                                      .OrderByDescending(row => row.TijdstipToegewezen)
                                      .Select(row => row.Tafel.Nummer)
                                      .FirstOrDefaultAsync();
            var currentVisitor = await _bestelplatformContext.Bezoekers
                                             .Where(row => row.Gebruiker.UniekeCode == cookieToken)
                                             .FirstOrDefaultAsync();
            if (!await CheckIfUserExists(currentVisitor))
            {
                return View("Foutpagina");
            }
            var model = new overzichtPaginaViewModel();
            model.TableNumber = tableNumber;
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
            return View("OverzichtPagina", model);
        }

        [HttpGet("betaling")]
        public async Task<IActionResult> Betaling()
        {
            string? cookieToken = Request.Cookies["UserCookie"];
            if (cookieToken == null)
            {
                return View("FoutPagina");
            }
            else
            {
                var tableNumber = await _bestelplatformContext.Tafeltoewijzingens
                                    .Where(row => row.Gebruiker.Gebruiker.UniekeCode == cookieToken)
                                    .Select(row => row.Tafel.Nummer)
                                    .FirstOrDefaultAsync();
                return RedirectToAction("index", new { tafelnummer = tableNumber });
            }
        }

        [HttpPost("betaling")]
        public async Task<IActionResult> Betaling(float totalPrice, List<OrderedProductProperties> orderedProductProperties)
        {
            string? cookieToken = Request.Cookies["UserCookie"];
            if (cookieToken == null)
            {
                return View("FoutPagina");
            }
            var tableNumber = await _bestelplatformContext.Tafeltoewijzingens
                                   .Where(row => row.Gebruiker.Gebruiker.UniekeCode == cookieToken)
                                   .Select(row => row.Tafel.Nummer)
                                   .FirstOrDefaultAsync();
            var currentVisitor = await _bestelplatformContext.Bezoekers
                                             .Where(row => row.Gebruiker.UniekeCode == cookieToken)
                                             .FirstOrDefaultAsync();
            if (!await CheckIfUserExists(currentVisitor))
            {
                return View("Foutpagina");
            }
            //Mollie testbetaling laden.
            string orderedProductPropertiesJson = JsonSerializer.Serialize(orderedProductProperties);
            using var paymentClient = new PaymentClient(_mollieApiKey);
            string huidigeBasisUrl = $"{Request.Scheme}://{Request.Host}";
            var paymentRequest = new PaymentRequest
            {
                Amount = new Amount(Currency.EUR, totalPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)),
                Description = $"Test ORW Wielewaal - Tafel {tableNumber}",
                RedirectUrl = $"{huidigeBasisUrl}/bestel/status",
                Metadata = orderedProductPropertiesJson
            };
            PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);
            if (paymentResponse == null)
            {
                return RedirectToAction("index", new { tafelnummer = tableNumber });
            }
            var updateRequest = new PaymentUpdateRequest
            {
                RedirectUrl = $"{huidigeBasisUrl}/bestel/status?paymentid={paymentResponse.Id}"
            };
            await paymentClient.UpdatePaymentAsync(paymentResponse.Id, updateRequest);
            return Redirect(paymentResponse.Links!.Checkout!.Href);
        }
        [HttpGet("status")]
        public async Task<IActionResult> Status(string paymentid)
        {
            string? cookieToken = Request.Cookies["UserCookie"];
            if (cookieToken == null)
            {
                return View("FoutPagina");
            }
            else
            {
                var model = new statusPaginaViewModel();
                var visitorData = await _bestelplatformContext.Bezoekers
                                        .Where(row => row.Gebruiker.UniekeCode == cookieToken)
                                        .Select(row => new
                                        {
                                            latestTableNumber = row.Tafeltoewijzingens
                                                                .OrderByDescending(row => row.TijdstipToegewezen)
                                                                .Select(row => row.Tafel.Nummer)
                                                                .FirstOrDefault(),
                                            activeUser = row,
                                            latestPaymentId = row.Bestellingens
                                                              .OrderByDescending(row => row.TijdstipBesteld)
                                                              .Select(row => row.MolliePaymentid)
                                                              .FirstOrDefault()
                                        })
                                        .FirstOrDefaultAsync();
                var currentVisitor = await _bestelplatformContext.Bezoekers
                                             .Where(row => row.Gebruiker.UniekeCode == cookieToken)
                                             .FirstOrDefaultAsync();
                if (!await CheckIfUserExists(currentVisitor))
                {
                    return View("Foutpagina");
                }
                var latestPaymentid = visitorData.latestPaymentId;
                if (latestPaymentid == null || latestPaymentid != paymentid)
                {
                    if (paymentid != null)
                    {
                        using var paymentClient = new PaymentClient(_mollieApiKey);
                        PaymentResponse payment = await paymentClient.GetPaymentAsync(paymentid);
                        if (payment.Status == PaymentStatus.Paid)
                        {
                            // Na betaling de tabellen bestellingen en bestellijnen invullen.
                            // Record aanmaken voor bestellingen.
                            var newOrder = new Bestellingen
                            {
                                Gebruiker = visitorData.activeUser,
                                TijdstipBesteld = DateTime.Now,
                                Status = "besteld",
                                MolliePaymentid = paymentid
                            };
                            _bestelplatformContext.Add(newOrder);
                            await _bestelplatformContext.SaveChangesAsync();
                            // Record aanmaken voor bestellijnen.
                            String jsonText = (string)payment.Metadata;
                            List<OrderedProductProperties> orderedProductProperties = JsonSerializer.Deserialize<List<OrderedProductProperties>>(jsonText)!;
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
                        }
                    }
                }
                // De bestelling meegeven aan een model.
                model.TableNumber = visitorData.latestTableNumber;
                model.orderDetails = await _bestelplatformContext.Bestellingens
                                       .Where(row => row.Gebruiker.Gebruiker.UniekeCode == cookieToken)
                                       .OrderByDescending(row => row.TijdstipBesteld)
                                       .Select(row => new OrderDetails()
                                       {
                                           Status = row.Status,
                                           orderedProductDetails = row.Bestellijnens
                                                                   .Select(row => new OrderedProductsDetails
                                                                   {
                                                                       ProductName = row.Product.Productdetails
                                                                                     .OrderByDescending(row => row.Tijdstip)
                                                                                     .Select(row => row.Naam)
                                                                                     .FirstOrDefault(),
                                                                       Amount = row.Hoeveelheid
                                                                   })
                                                                   .ToList()
                                       })
                                       .ToListAsync();
                return View("StatusPagina", model);
            }
        }
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
        public string? ProductName { get; set; }
        public int ProductID { get; set; }
    }
}
