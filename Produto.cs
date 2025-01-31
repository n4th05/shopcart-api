namespace ShopCartAPI
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }
        public string UnidadeDeMedida { get; set; }

        public int GetQuantidade()
        {
            return Quantidade;
        }

        public void SetQuantidade(int quantidade)
        {
            Quantidade = quantidade;
        }

        public string GetUnidadeDeMedida()
        {
            return UnidadeDeMedida;
        }

        public void SetUnidadeDeMedida(string unidadeDeMedida)
        {
            UnidadeDeMedida = unidadeDeMedida;
        }

        public Produto GetProduto()
        {
            return Produto;
        }

        public void SetProduto(Produto produto)
        {
            Produto = produto;
        }
    }

    public class Carrinho
    {
        public int Id { get; set; }
        public List<Item> ItensCarrinho { get; set; }

        public Carrinho()
        {
            ItensCarrinho = new List<Item>();
        }

        public int GetId()
        {
            return Id;
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public List<Item> GetItensCarrinho()
        {
            return ItensCarrinho;
        }

        public void SetItensCarrinho(List<Item> itens)
        {
            ItensCarrinho = itens;
        }

        public void AdicionarItem(Item item)
        {
            ItensCarrinho.Add(item);
        }

        public void RemoverItem(Item item)
        {
            ItensCarrinho.Remove(item);
        }
    }
}
