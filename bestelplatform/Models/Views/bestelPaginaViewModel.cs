namespace bestelplatform.Models.Views
{
    public class bestelPaginaViewModel
    {
        public int TafelNummer { get; set; }
        public List<String> ProductNamen { get; set; } = [];
        public List<int> IDs { get; set; } = [];
    }
}
