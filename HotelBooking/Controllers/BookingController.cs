using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models.Entities;
using HotelBooking.Models.ViewModels;

namespace HotelBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Booking/Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout(int hotelId, int roomId, DateTime? checkIn, DateTime? checkOut, int guests = 2, int quantity = 1)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId && h.IsActive);
            var room = await _context.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId && r.IsActive);

            if (hotel == null || room == null)
            {
                TempData["ErrorMessage"] = "Thông tin phòng hoặc khách sạn không tồn tại.";
                return RedirectToAction("Index", "Hotel");
            }

            var checkInDate = checkIn ?? DateTime.Today.AddDays(1);
            var checkOutDate = checkOut ?? DateTime.Today.AddDays(2);
            if (checkOutDate <= checkInDate)
            {
                checkOutDate = checkInDate.AddDays(1);
            }

            var model = new CheckoutViewModel
            {
                HotelId = hotelId,
                Hotel = hotel,
                RoomId = roomId,
                Room = room,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                TotalGuests = Math.Max(1, guests),
                TotalRooms = Math.Max(1, Math.Min(quantity, room.AvailableQuantity)),
                UnitPrice = room.PricePerNight,
                PaymentMethod = "VNPay"
            };

            // Pre-fill user data if logged in
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    model.CustomerName = user.FullName;
                    model.CustomerEmail = user.Email ?? string.Empty;
                    model.CustomerPhone = user.PhoneNumber ?? string.Empty;
                    model.CustomerAddress = user.Address;
                }
            }

            return View(model);
        }

        // POST: /Booking/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == model.HotelId);
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == model.RoomId && r.HotelId == model.HotelId);

            if (hotel == null || room == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phòng tương ứng.";
                return RedirectToAction("Index", "Hotel");
            }

            model.Hotel = hotel;
            model.Room = room;
            model.UnitPrice = room.PricePerNight;

            if (model.CheckOutDate <= model.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Ngày trả phòng phải sau ngày nhận phòng.");
            }

            if (model.CheckInDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("CheckInDate", "Ngày nhận phòng không thể ở quá khứ.");
            }

            if (room.AvailableQuantity < model.TotalRooms)
            {
                ModelState.AddModelError(string.Empty, $"Phòng {room.Name} hiện chỉ còn {room.AvailableQuantity} phòng trống.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Generate Booking Code
            string dateStr = DateTime.UtcNow.ToString("yyyyMMdd");
            string randomStr = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
            string bookingCode = $"HB-{dateStr}-{randomStr}";

            string? userId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            int nights = Math.Max(1, (model.CheckOutDate.Date - model.CheckInDate.Date).Days);
            decimal subTotal = room.PricePerNight * nights * model.TotalRooms;
            decimal tax = Math.Round(subTotal * 0.08m, 0);
            decimal fee = Math.Round(subTotal * 0.05m, 0);
            decimal total = subTotal + tax + fee;

            var booking = new Booking
            {
                BookingCode = bookingCode,
                UserId = userId,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                CustomerAddress = model.CustomerAddress,
                HotelId = model.HotelId,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                TotalGuests = model.TotalGuests,
                TotalRooms = model.TotalRooms,
                SubTotal = subTotal,
                TaxAmount = tax,
                ServiceFee = fee,
                TotalAmount = total,
                Status = "Pending",
                Notes = model.Notes,
                CreatedAt = DateTime.UtcNow,
                BookingDetails = new List<BookingDetail>
                {
                    new BookingDetail
                    {
                        RoomId = room.Id,
                        RoomName = room.Name,
                        PricePerNight = room.PricePerNight,
                        Quantity = model.TotalRooms,
                        Nights = nights,
                        SubTotal = subTotal
                    }
                }
            };

            // Deduct available quantity temporarily
            room.AvailableQuantity = Math.Max(0, room.AvailableQuantity - model.TotalRooms);

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Redirect to Payment Screen
            return RedirectToAction("Payment", new { id = booking.Id, method = model.PaymentMethod });
        }

        // GET: /Booking/Payment/5
        [HttpGet]
        public async Task<IActionResult> Payment(int id, string method = "VNPay")
        {
            var booking = await _context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var model = new PaymentDemoViewModel
            {
                BookingId = booking.Id,
                BookingCode = booking.BookingCode,
                HotelName = booking.Hotel?.Name ?? "Khách sạn",
                RoomName = booking.BookingDetails.FirstOrDefault()?.RoomName ?? "Phòng nghỉ",
                TotalAmount = booking.TotalAmount,
                PaymentMethod = method,
                CustomerName = booking.CustomerName,
                CustomerPhone = booking.CustomerPhone
            };

            return View(model);
        }

        // POST: /Booking/ProcessPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int bookingId, string paymentMethod)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            string payCode = $"PAY-{paymentMethod.ToUpper()}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            string txnRef = $"TXN{Random.Shared.Next(100000, 999999)}";

            if (booking.Payment == null)
            {
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    PaymentCode = payCode,
                    Amount = booking.TotalAmount,
                    PaymentMethod = paymentMethod,
                    Status = (paymentMethod == "Cash") ? "Pending" : "Completed",
                    TransactionRef = txnRef,
                    PaidAt = (paymentMethod == "Cash") ? null : DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Payments.AddAsync(payment);
            }
            else
            {
                booking.Payment.PaymentMethod = paymentMethod;
                booking.Payment.Status = (paymentMethod == "Cash") ? "Pending" : "Completed";
                booking.Payment.TransactionRef = txnRef;
                booking.Payment.PaidAt = (paymentMethod == "Cash") ? null : DateTime.UtcNow;
            }

            // If online paid, auto-confirm booking
            if (paymentMethod != "Cash")
            {
                booking.Status = "Confirmed";
            }
            else
            {
                booking.Status = "Pending";
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thanh toán đơn đặt phòng thành công!";
            return RedirectToAction("Success", new { id = booking.Id });
        }

        // GET: /Booking/Success/5
        [HttpGet]
        public async Task<IActionResult> Success(int id)
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

        // GET: /Booking/History
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.Payment)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Room)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bookings);
        }

        // GET: /Booking/Details/5
        [HttpGet]
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

        // POST: /Booking/Cancel/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(b => b.Id == id && (b.UserId == userId || User.IsInRole("Admin") || User.IsInRole("Staff")));

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn đặt phòng hoặc bạn không có quyền hủy.";
                return RedirectToAction("History");
            }

            if (booking.Status == "CheckedIn" || booking.Status == "CheckedOut" || booking.Status == "Completed")
            {
                TempData["ErrorMessage"] = "Đơn đặt phòng đang trong quá trình sử dụng hoặc đã hoàn tất, không thể hủy.";
                return RedirectToAction("History");
            }

            booking.Status = "Cancelled";

            // Restore room available quantity
            foreach (var detail in booking.BookingDetails)
            {
                var room = await _context.Rooms.FindAsync(detail.RoomId);
                if (room != null)
                {
                    room.AvailableQuantity += detail.Quantity;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã hủy thành công đơn đặt phòng {booking.BookingCode}.";
            return RedirectToAction("History");
        }
    }
}
