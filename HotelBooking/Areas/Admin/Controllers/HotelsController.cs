using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models.Entities;

namespace HotelBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Hotels
        public async Task<IActionResult> Index(string? search, string? city)
        {
            var query = _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(h => h.Name.ToLower().Contains(s) || h.Address.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(h => h.City == city);
            }

            var hotels = await query.OrderByDescending(h => h.CreatedAt).ToListAsync();
            ViewData["Cities"] = await _context.Hotels.Select(h => h.City).Distinct().ToListAsync();
            ViewData["CurrentSearch"] = search;
            ViewData["CurrentCity"] = city;

            return View(hotels);
        }

        // GET: /Admin/Hotels/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomType)
                .Include(h => h.HotelAmenities)
                    .ThenInclude(ha => ha.Amenity)
                .Include(h => h.Reviews)
                .Include(h => h.Bookings)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // GET: /Admin/Hotels/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Amenities = await _context.Amenities.ToListAsync();
            return View(new Hotel { StarRating = 5, IsActive = true, IsFeatured = true });
        }

        // POST: /Admin/Hotels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel, List<int> selectedAmenities)
        {
            if (ModelState.IsValid)
            {
                hotel.Slug = hotel.Name.ToLower().Replace(" ", "-").Replace("/", "-");
                hotel.CreatedAt = DateTime.UtcNow;

                await _context.Hotels.AddAsync(hotel);
                await _context.SaveChangesAsync();

                if (selectedAmenities != null && selectedAmenities.Any())
                {
                    foreach (var amenId in selectedAmenities)
                    {
                        await _context.HotelAmenities.AddAsync(new HotelAmenity
                        {
                            HotelId = hotel.Id,
                            AmenityId = amenId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Thêm mới khách sạn \"{hotel.Name}\" thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Amenities = await _context.Amenities.ToListAsync();
            return View(hotel);
        }

        // GET: /Admin/Hotels/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.HotelAmenities)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
            {
                return NotFound();
            }

            ViewBag.Amenities = await _context.Amenities.ToListAsync();
            ViewBag.SelectedAmenityIds = hotel.HotelAmenities.Select(ha => ha.AmenityId).ToList();

            return View(hotel);
        }

        // POST: /Admin/Hotels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hotel model, List<int> selectedAmenities)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var hotel = await _context.Hotels
                    .Include(h => h.HotelAmenities)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (hotel == null)
                {
                    return NotFound();
                }

                hotel.Name = model.Name;
                hotel.Description = model.Description;
                hotel.Address = model.Address;
                hotel.City = model.City;
                hotel.Country = model.Country;
                hotel.StarRating = model.StarRating;
                hotel.Phone = model.Phone;
                hotel.Email = model.Email;
                hotel.MainImageUrl = model.MainImageUrl;
                hotel.IsFeatured = model.IsFeatured;
                hotel.IsActive = model.IsActive;

                // Update amenities
                _context.HotelAmenities.RemoveRange(hotel.HotelAmenities);
                if (selectedAmenities != null && selectedAmenities.Any())
                {
                    foreach (var amenId in selectedAmenities)
                    {
                        hotel.HotelAmenities.Add(new HotelAmenity
                        {
                            HotelId = hotel.Id,
                            AmenityId = amenId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật khách sạn \"{hotel.Name}\" thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Amenities = await _context.Amenities.ToListAsync();
            ViewBag.SelectedAmenityIds = selectedAmenities ?? new List<int>();
            return View(model);
        }

        // POST: /Admin/Hotels/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Bookings)
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel != null)
            {
                // Soft delete or hard delete if no bookings
                if (hotel.Bookings.Any())
                {
                    hotel.IsActive = false;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Khách sạn \"{hotel.Name}\" đã được chuyển sang trạng thái Tạm Ngưng (do đã có lịch sử đặt phòng).";
                }
                else
                {
                    _context.Hotels.Remove(hotel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã xóa vĩnh viễn khách sạn \"{hotel.Name}\"!";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
