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
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new Exception("User ID token मध्ये नाही.");

            return int.Parse(claim.Value);
        }


        // ==========================================
        // PLACE ORDER
        // ==========================================

        [Authorize(Roles = "User")]
        [HttpPost("place")]
        public IActionResult PlaceOrder(int addressId)
        {
            var userId = GetUserId();


            // Check Address
            var address = _context.Addresses
                .FirstOrDefault(a =>
                    a.Id == addressId &&
                    a.UserId == userId);

            if (address == null)
                return BadRequest(
                    "Please select a valid delivery address."
                );


            // Get Cart
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId);


            if (cart == null || !cart.CartItems.Any())
                return BadRequest("Cart is empty.");


            // Create Order
            var order = new Order
            {
                UserId = userId,

                AddressId = addressId,

                OrderDate = DateTime.Now,

                TotalAmount = cart.CartItems.Sum(
                    ci => ci.Quantity * ci.Product.Price
                ),

                Status = "Pending",

                OrderItems = new List<OrderItem>()
            };


            // Add Order Items
            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(
                    new OrderItem
                    {
                        ProductId = item.ProductId,

                        Quantity = item.Quantity,

                        Price = item.Product.Price
                    }
                );
            }


            _context.Orders.Add(order);


            // Clear Cart
            _context.CartItems.RemoveRange(
                cart.CartItems
            );


            _context.SaveChanges();


            return Ok(new
            {
                message = "Order placed successfully",

                orderId = order.Id
            });
        }


        // ==========================================
        // GET MY ORDERS
        // ==========================================

        [Authorize(Roles = "User")]
        [HttpGet("my-orders")]
        public IActionResult GetMyOrders()
        {
            var userId = GetUserId();

            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status,

                    Items = o.OrderItems.Select(i => new
                    {
                        i.ProductId,
                        ProductName = i.Product.Name,
                        ImageUrl = i.Product.ImageUrl,
                        i.Quantity,
                        i.Price,
                        Total = i.Quantity * i.Price
                    })
                })
                .ToList();

            return Ok(orders);
        }


        // ==========================================
        // UPDATE ORDER STATUS
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPut("update-status")]
        public IActionResult UpdateOrderStatus(
            int orderId,
            string status)
        {
            var order = _context.Orders
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found.");


            var validStatus = new[]
            {
                "Pending",
                "Confirmed",
                "Shipped",
                "Delivered"
            };


            if (!validStatus.Contains(status))
                return BadRequest("Invalid status.");


            order.Status = status;

            _context.SaveChanges();


            return Ok(new
            {
                message = "Status updated successfully",

                newStatus = status
            });
        }
    }
}