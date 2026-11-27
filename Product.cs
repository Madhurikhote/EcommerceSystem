using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }   // 🔍 For search

        [Required]
        public string Category { get; set; }       // 🧭 For browse/filter

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? Brand { get; set; }         // ⭐ Optional filter

        public string? ImageUrl { get; set; }      // 🖼️ For frontend

        public bool IsAvailable { get; set; } = true;  // ✅ Active/Inactive

        public DateTime CreatedAt { get; set; } = DateTime.Now; // 🕒 Sorting

        public double Rating { get; set; } = 0;    // ⭐ Future (reviews)
    }
}
