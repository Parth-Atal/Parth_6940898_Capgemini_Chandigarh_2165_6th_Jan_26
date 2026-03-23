using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        public string Type { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Amenities { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}