using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(1, 100000)]
        public decimal Price { get; set; }

        public string Category { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}