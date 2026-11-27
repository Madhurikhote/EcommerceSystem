using System.Text.Json.Serialization;

namespace EcommerceWebAPI.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public DateTime AddedAt { get; set; } = DateTime.Now;
        
    }
}
