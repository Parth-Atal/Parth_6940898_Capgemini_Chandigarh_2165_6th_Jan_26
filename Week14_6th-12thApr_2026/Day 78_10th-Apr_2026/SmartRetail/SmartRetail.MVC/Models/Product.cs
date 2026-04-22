using System.ComponentModel.DataAnnotations;

namespace SmartRetail.MVC.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 99999.99)]
        public decimal Price { get; set; }
    }
}