using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        static private List<Produto> produtos = new List<Produto>
        {
            new Produto
            {
                Id = 1,
                Nome = "Cadeira Gamer"
            },
            new Produto
            {
                Id = 2,
                Nome = "Mouse Sem Fio"
            },
            new Produto
            {
                Id = 3,
                Nome = "Teclado Mecânico"
            },
            new Produto
            {
                Id = 4,
                Nome = "Monitor 4K"
            },
            new Produto
            {
                Id = 5,
                Nome = "Notebook Ultrafino"
            }
        };
        [HttpGet]
        public ActionResult<List<Produto>> GetProdutos()
        {
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public ActionResult<Produto> GetProdutoById(int id)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto is null)
                return NotFound();

            return Ok(produto);
        }

        [HttpPost]
        public ActionResult<Produto> AddProduto(Produto newProduto)
        {
            if (newProduto is null)
                return BadRequest();

            newProduto.Id = produtos.Max(p => p.Id) + 1;
            produtos.Add(newProduto);
            return CreatedAtAction(nameof(GetProdutoById), new { id = newProduto.Id }, newProduto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduto(int id, Produto updatesProduto)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            if (produto is null)
                return NotFound();

            produto.Nome = updatesProduto.Nome;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduto(int id)
        {
            {
                var produto = produtos.FirstOrDefault(p => p.Id == id);
                if (produto is null)
                    return NotFound();

                produtos.Remove(produto);
                return NoContent();
            }
        }
    }
}
