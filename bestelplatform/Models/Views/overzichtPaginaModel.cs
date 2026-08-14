using bestelplatform.Controllers;
using Org.BouncyCastle.Bcpg;

namespace bestelplatform.Models.Views
{
    public class overzichtPaginaModel
    {
        public int TableNumber { get; set; }
        public List<OrderInputProperties> ?BestelInputProperties { get; set; }
        public List<OrderedProductProperties> ?BesteldProductProperties { get; set; }
        public float TotalPrice { get; set; }
    }
}
