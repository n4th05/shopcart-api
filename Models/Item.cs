namespace ShopCartAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public Produto? Produto { get; set; }
        public Carrinho? Carrinho { get; set; }
        public int? Quantidade { get; set; }
        public string? UnidadeDeMedida { get; set; }
    }
}