using EcommerceWebAPI.Data;
using EcommerceWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")] // ✅ ONLY USER
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Helper Method
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        // ✅ 1. Add To Cart
        [HttpPost("add")]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = GetUserId();

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
                return NotFound("Product not found");

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });

            _context.SaveChanges();

            return Ok("Product added to cart");
        }

        // ✅ 2. Get Cart
        [HttpGet]
        public IActionResult GetCart()
        {
            var userId = GetUserId();

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
                return NotFound("Cart is empty");
            var result = new
            {
                cart.Id,

                Items = cart.CartItems.Select(ci => new
                {
                    ci.Id,
                    ci.ProductId,
                    ProductName = ci.Product.Name,
                    ImageUrl = ci.Product.ImageUrl,
                    ci.Quantity,
                    Price = ci.Product.Price,
                    Total = ci.Quantity * ci.Product.Price
                }),

                GrandTotal = cart.CartItems.Sum(
                    ci => ci.Quantity * ci.Product.Price)
            };

            return Ok(result);
        }

        // ✅ 3. Update Quantity
        [HttpPut("update")]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            var item = _context.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

            if (item == null)
                return NotFound("Cart item not found");

            item.Quantity = quantity;
            _context.SaveChanges();

            return Ok("Quantity updated");
        }

        // ✅ 4. Remove Item
        [HttpDelete("remove/{id}")]
        public IActionResult RemoveItem(int id)
        {
            var item = _context.CartItems.FirstOrDefault(ci => ci.Id == id);

            if (item == null)
                return NotFound("Item not found");

            _context.CartItems.Remove(item);
            _context.SaveChanges();

            return Ok("Item removed");
        }
    }
}
