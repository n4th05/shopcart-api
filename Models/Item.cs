namespace ShopCartAPI.Models
{
    public class Item
    {
        public Produto Produto { get; set; }
        public int ProdutoId { get; set; }
        public int CarrinhoId { get; set; }
        public int Quantidade { get; set; }
        public string UnidadeDeMedida { get; set; }
    }
}