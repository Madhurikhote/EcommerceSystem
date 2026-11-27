using EcommerceWebAPI.Data;
using EcommerceWebAPI.DTOs;
using EcommerceWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // 🔓 PUBLIC - Get All Products
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        // 🔓 PUBLIC - Get Product By Id
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // 🔐 ADMIN - Add Product
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddProduct(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Price = dto.Price,
                Stock = dto.Stock,
                Brand = dto.Brand,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.Now,
                IsAvailable = true
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }

        // 🔐 ADMIN - Update Product
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, UpdateProductDto dto)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            if (dto.Name != null) product.Name = dto.Name;
            if (dto.Description != null) product.Description = dto.Description;
            if (dto.Category != null) product.Category = dto.Category;
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;
            if (dto.Brand != null) product.Brand = dto.Brand;
            if (dto.ImageUrl != null) product.ImageUrl = dto.ImageUrl;
            if (dto.IsAvailable.HasValue) product.IsAvailable = dto.IsAvailable.Value;

            _context.SaveChanges();

            return Ok(product);
        }

        // 🔐 ADMIN - Delete Product
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound("Product not found");

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok("Product deleted successfully");
        }

        // 🔍 SEARCH + BROWSE (🔥 MOST IMPORTANT)
        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] ProductSearchDto param)
        {
            var query = _context.Products.AsQueryable();

            // 🔍 Keyword
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                query = query.Where(p =>
                    p.Name.Contains(param.Keyword) ||
                    p.Description.Contains(param.Keyword));
            }

            // 📂 Category
            if (!string.IsNullOrEmpty(param.Category))
            {
                query = query.Where(p => p.Category == param.Category);
            }

            // 🏷️ Brand
            if (!string.IsNullOrEmpty(param.Brand))
            {
                query = query.Where(p => p.Brand == param.Brand);
            }

            // 💰 Price
            if (param.MinPrice.HasValue)
                query = query.Where(p => p.Price >= param.MinPrice);

            if (param.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= param.MaxPrice);

            // 🔄 Sorting
            query = param.Sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "latest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            // 📄 Pagination
            var total = query.Count();

            var products = query
                .Skip((param.Page - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToList();

            return Ok(new
            {
                total,
                page = param.Page,
                pageSize = param.PageSize,
                data = products
            });
        }

        // 📂 CATEGORY LIST
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            return Ok(categories);
        }
    }
}
