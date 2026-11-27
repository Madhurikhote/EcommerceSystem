using EcommerceWebAPI.Data;
using EcommerceWebAPI.DTOs;
using EcommerceWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")] // Only users
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        // Get userId from JWT
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new Exception("Token me user id nahi hai");
            return int.Parse(claim.Value);
        }

        // -----------------
        // Add to wishlist
        // -----------------
        [HttpPost]
        public IActionResult AddToWishlist(CreateWishlistDTO dto)
        {
            int userId = GetUserId();

            // check if already added
            var existing = _context.Wishlists
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == dto.ProductId);
            if (existing != null)
                return BadRequest("Product already in wishlist");

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = dto.ProductId,
                ProductName = dto.ProductName,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl
            };

            _context.Wishlists.Add(wishlistItem);
            _context.SaveChanges();

            return Ok(wishlistItem);
        }

        // -----------------
        // Get user wishlist
        // -----------------
        [HttpGet]
        public IActionResult GetWishlist()
        {
            int userId = GetUserId();
            var wishlist = _context.Wishlists
                .Where(w => w.UserId == userId)
                .ToList();
            return Ok(wishlist);
        }

        // -----------------
        // Remove from wishlist
        // -----------------
        [HttpDelete("{id}")]
        public IActionResult RemoveFromWishlist(int id)
        {
            int userId = GetUserId();

            var item = _context.Wishlists
                .FirstOrDefault(w => w.Id == id && w.UserId == userId);

            if (item == null)
                return NotFound("Wishlist item nahi mila");

            _context.Wishlists.Remove(item);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}
