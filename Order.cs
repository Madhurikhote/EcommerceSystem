using System.Text.Json.Serialization;

namespace EcommerceWebAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        public int? AddressId { get; set; }

        public Address? Address { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}