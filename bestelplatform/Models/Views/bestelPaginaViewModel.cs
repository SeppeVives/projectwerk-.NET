using bestelplatform.Controllers;
using bestelplatform.DTOs;

namespace bestelplatform.Models.Views
{
    public class bestelPaginaViewModel
    {
        public int TableNumber { get; set; }
        public List<ProductdetailsDto> ProductDetails { get; set; } = [];
    }
}
