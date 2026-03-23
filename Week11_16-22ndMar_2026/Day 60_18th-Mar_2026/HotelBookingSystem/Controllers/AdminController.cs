using HotelBookingSystem;
using Microsoft.AspNetCore.Mvc;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        ViewBag.TotalRooms = _context.Rooms.Count();
        ViewBag.TotalBookings = _context.Bookings.Count();

        return View();
    }
}