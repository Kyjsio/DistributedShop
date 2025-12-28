using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs;
using ProductService.Entities;

namespace ProductService.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductDbContext _context;
        public ProductController(ProductDbContext context)
        {
            _context = context;
        }
  
        [HttpGet("getProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var results = await _context.Products.ToListAsync();
            return results;
        }


        [HttpPost("createProducts")]
        public async Task<ActionResult<Product>> Create(CreateProductDto productDto)
        {
           var product = new Product
           {
               Name = productDto.Name,
               Description = productDto.Description,
               Price = productDto.Price,
               StockQuantity = productDto.StockQuantity
           };
              _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        [HttpPut("updateProducts/{id}")]
        public  async Task<ActionResult> UpdateProduct(int id, CreateProductDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            product.StockQuantity += product.StockQuantity;
            product.Price = productDto.Price;
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("deleteProducts/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
