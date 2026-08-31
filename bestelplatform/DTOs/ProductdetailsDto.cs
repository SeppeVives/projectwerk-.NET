namespace bestelplatform.DTOs
{
    public class ProductdetailsDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int ProductID { get; set; }
        public float ProductPrice { get; set; }
        public String? ProductType { get; set; }
    }
}
