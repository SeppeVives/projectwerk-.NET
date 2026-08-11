using bestelplatform.Controllers;
using Org.BouncyCastle.Bcpg;

namespace bestelplatform.Models.Views
{
    public class overzichtPaginaModel
    {
        public int Tafelnummer { get; set; }
        public List<BestelInputProperties> BestelInputProperties { get; set; }
        public List<BesteldProductProperties> BesteldProductProperties { get; set; }
        public float TotaalPrijs { get; set; }
    }
}
