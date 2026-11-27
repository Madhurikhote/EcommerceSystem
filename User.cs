using System.Text.Json.Serialization;

namespace EcommerceWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        [JsonIgnore]
        public List<Order> Orders { get; set; } = new List<Order>();

        [JsonIgnore]
        public List<Address> Addresses { get; set; } = new List<Address>();
    }
}