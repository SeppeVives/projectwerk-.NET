using bestelplatform.Models.Views;
using bestelplatform.Data.bestelplatform; // database
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bestelplatform.Controllers
{
    [Route("[controller]")]
    public class bestelController : Controller
    {
        private readonly BestelplatformContext _bestelplatformContext;
        
        public bestelController(BestelplatformContext bestelplatformContext)
        {
            _bestelplatformContext = bestelplatformContext;
        }

        [HttpGet("")]

        public async Task<IActionResult> Index(int tafelnummer = 0)
        {
            var model = new bestelPaginaViewModel();
            model.TafelNummer = tafelnummer;
            model.ProductNamen = await _bestelplatformContext.Productdetails
                .Select(tabel => tabel.Naam)
                .ToListAsync();
            model.IDs = await _bestelplatformContext.Productdetails
                .Select(tabel => tabel.ProductId)
                .ToListAsync();

            return View("bestelpagina", model);
        }

        [HttpPost("overzicht")]
        public async Task<IActionResult> overzicht(List<BestelInputProperties> bestelInputs, int tafelNummer)
        {
            var model = new overzichtPaginaModel();
            model.Tafelnummer = tafelNummer;
            var productIDs = await _bestelplatformContext.Productdetails
                .Select(tabel => tabel.ProductId)
                .ToListAsync();
            var besteldProductProperties = new List<BesteldProductProperties>();
            float totaalPrijs = 0;
            // De verstuurde ID's van de form filteren (groter dan 0) met LINQ.
            var filteredIDs = bestelInputs
                .Where(x => x.Amount > 0)
                .Select(x => x.InputID)
                .ToList();
            var eenheidsprijzen = await _bestelplatformContext.Productdetails
                .Where(tabel => filteredIDs.Contains(tabel.ProductId))
                .ToListAsync();

            foreach (var bestelInput in bestelInputs)
            {
                if (bestelInput.Amount > 0)
                {
                    var eenheidsPrijs = eenheidsprijzen
                        .Where(row => row.ProductId == bestelInput.InputID)
                        .Select(row => row.Prijs)
                        .FirstOrDefault();
                    var subtotaalPrijs = bestelInput.Amount * eenheidsPrijs;
                    besteldProductProperties.Add(new BesteldProductProperties
                    {
                        Eenheidsprijs = eenheidsPrijs,
                        SubtotaalPrijs = subtotaalPrijs,
                        Hoeveelheid = bestelInput.Amount,
                        ProductNaam = bestelInput.ProductName
                    });
                    totaalPrijs += subtotaalPrijs;
                }
            }
            model.BestelInputProperties = bestelInputs;
            model.BesteldProductProperties = besteldProductProperties;
            model.TotaalPrijs = totaalPrijs;
            return View("overzichtPagina", model);
        }
    }
    public class BestelInputProperties
    {
        public int InputID { get; set; }
        public int Amount { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
    public class BesteldProductProperties
    {
        public float Eenheidsprijs { get; set; }
        public float SubtotaalPrijs { get; set; }
        public int Hoeveelheid {  get; set; }
        public string ProductNaam {  get; set; } = string.Empty;
    }
}
