namespace ShopCartAPI.Models
{
    public class Carrinho
    {
        public int Id { get; set; }
        public List<Item>? ItensCarrinho { get; set; } = new List<Item>();
    }
}