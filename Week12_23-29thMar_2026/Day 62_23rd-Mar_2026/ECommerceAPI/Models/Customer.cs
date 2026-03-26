using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}