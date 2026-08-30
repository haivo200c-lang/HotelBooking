using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models.ViewModels;

namespace HotelBooking.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Room/Detail/5
        public async Task<IActionResult> Detail(int id, DateTime? checkIn, DateTime? checkOut, int guests = 2, int quantity = 1)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Images)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .Include(r => r.Hotel)
                    .ThenInclude(h => h!.Images)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

            if (room == null || room.Hotel == null)
            {
                return NotFound();
            }

            var checkInDate = checkIn ?? DateTime.Today.AddDays(1);
            var checkOutDate = checkOut ?? DateTime.Today.AddDays(2);
            if (checkOutDate <= checkInDate)
            {
                checkOutDate = checkInDate.AddDays(1);
            }

            var viewModel = new RoomDetailViewModel
            {
                Room = room,
                Hotel = room.Hotel,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                Guests = Math.Max(1, Math.Min(guests, room.Capacity * quantity)),
                Quantity = Math.Max(1, Math.Min(quantity, room.AvailableQuantity))
            };

            return View(viewModel);
        }
    }
}
