using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        static private List<Item> itens = new List<Item>
        {
            new Item
            {
                Id = 1,
                Produto = new Produto { Id = 1, Nome = "Capa" },
                UnidadeDeMedida = "unidade",
                Quantidade = 5
            },
            new Item
            {
                Id = 2,
                Produto = new Produto { Id = 2, Nome = "Mouse Sem Fio" },
                UnidadeDeMedida = "unidade",
                Quantidade = 10
            },
            new Item
            {
                Id = 3,
                Produto = new Produto { Id = 3, Nome = "Teclado Mecânico" },
                UnidadeDeMedida = "unidade",
                Quantidade = 7
            },
            new Item
            {
                Id = 4,
                Produto = new Produto { Id = 4, Nome = "Monitor 4K" },
                UnidadeDeMedida = "unidade",
                Quantidade = 3
            },
            new Item
            {
                Id = 5,
                Produto = new Produto { Id = 5, Nome = "Notebook Ultrafino" },
                UnidadeDeMedida = "unidade",
                Quantidade = 2
            }
        };

        [HttpGet]
        public ActionResult<List<Item>> GetItens()
        {
            return Ok(itens);
        }

        [HttpGet("{id}")]
        public ActionResult<Item> GetItemById(int id)
        {
            var item = itens.FirstOrDefault(i => i.Id == id);
            if (item is null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public ActionResult<Item> AddItem(Item newItem)
        {
            if (newItem is null)
                return BadRequest();

            newItem.Id = itens.Max(i => i.Id) + 1;
            itens.Add(newItem);
            return CreatedAtAction(nameof(GetItemById), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItem(int id, Item updatesItem)
        {
            var item = itens.FirstOrDefault(i => i.Id == id);
            if (item is null)
                return NotFound();

            item.Produto = updatesItem.Produto;
            item.Quantidade = updatesItem.Quantidade;
            item.UnidadeDeMedida = updatesItem.UnidadeDeMedida;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            var item = itens.FirstOrDefault(i => i.Id == id);
            if (item is null)
                return NotFound();

            itens.Remove(item);
            return NoContent();
        }
    }
}
