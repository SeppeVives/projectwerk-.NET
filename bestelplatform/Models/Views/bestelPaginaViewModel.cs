using bestelplatform.Controllers;

namespace bestelplatform.Models.Views
{
    public class bestelPaginaViewModel
    {
        public int TableNumber { get; set; }
        public List<ProductDetails> ProductDetails { get; set; } = [];
    }
}
