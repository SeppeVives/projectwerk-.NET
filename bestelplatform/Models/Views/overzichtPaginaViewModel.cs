using bestelplatform.Controllers;
using Org.BouncyCastle.Bcpg;

namespace bestelplatform.Models.Views
{
    public class overzichtPaginaViewModel
    {
        public int TableNumber { get; set; }
        public List<OrderInputProperties>? OrderInputProperties { get; set; }
        public List<OrderedProductProperties>? OrderedProductProperties { get; set; }
        public float TotalPrice { get; set; }
    }
}
