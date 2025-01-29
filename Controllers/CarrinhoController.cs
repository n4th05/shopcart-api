using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : ControllerBase
    {
        static private List<Carrinho> carrinhos = new List<Carrinho>
        {
            new Carrinho
            {
                Id = 1,
                ItensCarrinho = new List<Item>
                {
                    new Item { Id = 1, Produto = new Produto { Id = 1, Nome = "Cadeira Gamer" }, UnidadeDeMedida = "unidade", Quantidade = 1 }
                }
            },
            new Carrinho
            {
                Id = 2,
                ItensCarrinho = new List<Item>
                {
                    new Item { Id = 2, Produto = new Produto { Id = 2, Nome = "Mouse Sem Fio" }, UnidadeDeMedida = "unidade", Quantidade = 1 }
                }
            },
            new Carrinho
            {
                Id = 3,
                ItensCarrinho = new List<Item>
                {
                    new Item { Id = 3, Produto = new Produto { Id = 3, Nome = "Teclado Mecânico" }, UnidadeDeMedida = "unidade", Quantidade = 1 }
                }
            },
            new Carrinho
            {
                Id = 4,
                ItensCarrinho = new List<Item>
                {
                    new Item { Id = 4, Produto = new Produto { Id = 4, Nome = "Monitor 4K" }, UnidadeDeMedida = "unidade", Quantidade = 1 }
                }
            },
            new Carrinho
            {
                Id = 5,
                ItensCarrinho = new List<Item>
                {
                    new Item { Id = 5, Produto = new Produto { Id = 5, Nome = "Notebook Ultrafino" }, UnidadeDeMedida = "unidade", Quantidade = 1 }
                }
            }
        };

        [HttpGet]
        public ActionResult<List<Carrinho>> GetCarrinhos()
        {
            return Ok(carrinhos);
        }

        [HttpGet("{id}")]
        public ActionResult<Carrinho> GetCarrinhoById(int id)
        {
            var carrinho = carrinhos.FirstOrDefault(c => c.Id == id);
            if (carrinho is null)
                return NotFound();

            return Ok(carrinho);
        }

        [HttpPost]
        public ActionResult<Carrinho> AddCarrinho(Carrinho newCarrinho)
        {
            if (newCarrinho is null)
                return BadRequest();

            newCarrinho.Id = carrinhos.Max(c => c.Id) + 1;
            carrinhos.Add(newCarrinho);
            return CreatedAtAction(nameof(GetCarrinhoById), new { id = newCarrinho.Id }, newCarrinho);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCarrinho(int id, Carrinho updatesCarrinho)
        {
            var carrinho = carrinhos.FirstOrDefault(c => c.Id == id);
            if (carrinho is null)
                return NotFound();

            carrinho.ItensCarrinho = updatesCarrinho.ItensCarrinho;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCarrinho(int id)
        {
            var carrinho = carrinhos.FirstOrDefault(c => c.Id == id);
            if (carrinho is null)
                return NotFound();

            carrinhos.Remove(carrinho);
            return NoContent();
        }
    }
}
