using Microsoft.AspNetCore.Mvc;
using ShopCartAPI.Models;
using ShopCartAPI.Services.Interfaces;

namespace ShopCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : ControllerBase
    {
        private readonly ICarrinhoService _carrinhoService;

        public CarrinhoController(ICarrinhoService carrinhoService)
        {
            _carrinhoService = carrinhoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Carrinho>>> GetCarrinhos()
        {
            var carrinhos = await _carrinhoService.GetAllCarrinhosAsync();
            return Ok(carrinhos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Carrinho>> GetCarrinhoById(int id)
        {
            var carrinho = await _carrinhoService.GetCarrinhoByIdAsync(id, true);
            if (carrinho == null)
                return NotFound();
            return Ok(carrinho);
        }

        [HttpPost]
        public async Task<ActionResult<Carrinho>> AddCarrinho(Carrinho carrinho)
        {
            try
            {
                var createdCarrinho = await _carrinhoService.CreateCarrinhoAsync(carrinho);
                return createdCarrinho;
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCarrinho(int id, Carrinho carrinho)
        {
            try
            {
                await _carrinhoService.UpdateCarrinhoAsync(id, carrinho);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarrinho(int id)
        {
            try
            {
                await _carrinhoService.DeleteCarrinhoAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}