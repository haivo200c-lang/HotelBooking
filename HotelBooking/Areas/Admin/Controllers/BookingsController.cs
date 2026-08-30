using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;

namespace HotelBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Bookings
        public async Task<IActionResult> Index(string? status, string? search)
        {
            var query = _context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.Payment)
                .Include(b => b.BookingDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(b => b.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(b => b.BookingCode.ToLower().Contains(s) || b.CustomerName.ToLower().Contains(s) || b.CustomerPhone.Contains(s));
            }

            var bookings = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();

            ViewData["CurrentStatus"] = status;
            ViewData["CurrentSearch"] = search;

            return View(bookings);
        }

        // GET: /Admin/Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.Payment)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: /Admin/Bookings/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            string oldStatus = booking.Status;
            booking.Status = newStatus;

            // If changing to Cancelled, restore room available quantity
            if (newStatus == "Cancelled" && oldStatus != "Cancelled")
            {
                foreach (var detail in booking.BookingDetails)
                {
                    var room = await _context.Rooms.FindAsync(detail.RoomId);
                    if (room != null)
                    {
                        room.AvailableQuantity += detail.Quantity;
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn {booking.BookingCode} sang \"{newStatus}\"!";
            return RedirectToAction(nameof(Index));
        }
    }
}
