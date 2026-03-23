using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}