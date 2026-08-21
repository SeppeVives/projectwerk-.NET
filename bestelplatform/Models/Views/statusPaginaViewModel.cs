using bestelplatform.Controllers;
namespace bestelplatform.Models.Views
{
    public class statusPaginaViewModel
    {
        public int TableNumber { get; set; }
        public List<OrderDetails>? orderDetails { get; set; } = [];
    }
    public class OrderDetails
    {
        public string? Status { get; set; }
        public List<OrderedProductsDetails> orderedProductDetails { get; set; } = [];
    }

    public class OrderedProductsDetails
    {
        public string? ProductName { get; set; }
        public int Amount { get; set; }
    }
}
