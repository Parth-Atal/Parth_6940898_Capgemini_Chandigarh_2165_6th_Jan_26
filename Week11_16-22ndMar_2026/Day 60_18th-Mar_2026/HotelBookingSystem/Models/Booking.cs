using System.ComponentModel.DataAnnotations;

namespace HotelBookingSystem
{
    public class Booking
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}