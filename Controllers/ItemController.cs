using Microsoft.AspNetCore.Mvc;
using ShopCartAPI.Services;
using ShopCartAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: api/Item
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItens()
        {
            var items = await _itemService.GetAllItensAsync();
            return Ok(items);
        }

        // POST: api/Item
        [HttpPost]
        public async Task<ActionResult<Item>> AddItem([FromBody] Item newItem)
        {
            if (newItem == null)
                return BadRequest("Item data is required.");

            // Validação manual das propriedades do Item
            if (newItem.Produto == null || newItem.Produto.Id <= 0)
                return BadRequest("Produto ID is required and must be greater than 0.");

            if (newItem.Quantidade <= 0)
                return BadRequest("Quantidade must be greater than 0.");

            if (string.IsNullOrWhiteSpace(newItem.UnidadeDeMedida))
                return BadRequest("UnidadeDeMedida is required.");

            try
            {
                var createdItem = await _itemService.CreateItemAsync(newItem);
                return CreatedAtAction(nameof(GetItens), createdItem); // Alterado para retornar a lista de itens
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the item: {ex.Message}");
            }
        }

        // PUT: api/Item/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] Item updatedItem)
        {
            if (id <= 0)
                return BadRequest("ID must be greater than 0.");

            if (updatedItem == null)
                return BadRequest("Item data is required.");

            // Validação manual das propriedades do Item
            if (updatedItem.Produto == null || updatedItem.Produto.Id <= 0)
                return BadRequest("Produto ID is required and must be greater than 0.");

            if (updatedItem.Quantidade <= 0)
                return BadRequest("Quantidade must be greater than 0.");

            if (string.IsNullOrWhiteSpace(updatedItem.UnidadeDeMedida))
                return BadRequest("UnidadeDeMedida is required.");

            try
            {
                await _itemService.UpdateItemAsync(id, updatedItem);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the item: {ex.Message}");
            }
        }

        // DELETE: api/Item/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0)
                return BadRequest("ID must be greater than 0.");

            try
            {
                await _itemService.DeleteItemAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the item: {ex.Message}");
            }
        }
    }
}