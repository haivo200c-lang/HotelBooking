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
    public class HotelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HotelController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Hotel
        public async Task<IActionResult> Index(HotelSearchFilterViewModel filter)
        {
            var query = _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.HotelAmenities)
                    .ThenInclude(ha => ha.Amenity)
                .Include(h => h.Reviews)
                .Where(h => h.IsActive)
                .AsQueryable();

            // 1. Filter by Destination / City
            if (!string.IsNullOrWhiteSpace(filter.Destination))
            {
                var dest = filter.Destination.Trim().ToLower();
                query = query.Where(h => h.Name.ToLower().Contains(dest) 
                                      || h.City.ToLower().Contains(dest) 
                                      || h.Address.ToLower().Contains(dest));
            }

            if (!string.IsNullOrWhiteSpace(filter.City))
            {
                query = query.Where(h => h.City == filter.City);
            }

            // 2. Filter by Star Rating
            if (filter.StarRating.HasValue && filter.StarRating.Value > 0)
            {
                query = query.Where(h => h.StarRating == filter.StarRating.Value);
            }

            // 3. Filter by Amenities
            if (filter.SelectedAmenityIds != null && filter.SelectedAmenityIds.Any())
            {
                foreach (var amenityId in filter.SelectedAmenityIds)
                {
                    query = query.Where(h => h.HotelAmenities.Any(ha => ha.AmenityId == amenityId));
                }
            }

            // 4. Filter by Capacity / Guests
            if (filter.Guests > 0)
            {
                query = query.Where(h => h.Rooms.Any(r => r.IsActive && r.Capacity >= filter.Guests || r.Capacity * filter.Rooms >= filter.Guests));
            }

            // Fetch to memory for complex in-memory calculated fields (Price, Rating)
            var hotelList = await query.ToListAsync();

            // 5. Filter by Min/Max Price
            if (filter.MinPrice.HasValue)
            {
                hotelList = hotelList.Where(h => h.MinPrice >= filter.MinPrice.Value).ToList();
            }

            if (filter.MaxPrice.HasValue)
            {
                hotelList = hotelList.Where(h => h.MinPrice <= filter.MaxPrice.Value).ToList();
            }

            // 6. Filter by Min Rating Score
            if (filter.MinRating.HasValue)
            {
                hotelList = hotelList.Where(h => h.AverageRating >= filter.MinRating.Value).ToList();
            }

            // 7. Sorting
            hotelList = filter.SortBy switch
            {
                "price_asc" => hotelList.OrderBy(h => h.MinPrice).ToList(),
                "price_desc" => hotelList.OrderByDescending(h => h.MinPrice).ToList(),
                "rating_desc" => hotelList.OrderByDescending(h => h.AverageRating).ThenByDescending(h => h.TotalReviewCount).ToList(),
                _ => hotelList.OrderByDescending(h => h.IsFeatured).ThenByDescending(h => h.StarRating).ToList()
            };

            // 8. Pagination
            filter.TotalHotels = hotelList.Count;
            filter.Hotels = hotelList
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            // Load filters dropdown data
            filter.AvailableAmenities = await _context.Amenities.Take(12).ToListAsync();
            filter.AvailableCities = await _context.Hotels.Select(h => h.City).Distinct().ToListAsync();

            return View(filter);
        }

        // GET: /Hotel/Detail/5
        public async Task<IActionResult> Detail(int id, DateTime? checkIn, DateTime? checkOut, int guests = 2, int rooms = 1)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Images)
                .Include(h => h.HotelAmenities)
                    .ThenInclude(ha => ha.Amenity)
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomType)
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomAmenities)
                        .ThenInclude(ra => ra.Amenity)
                .Include(h => h.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(h => h.Id == id && h.IsActive);

            if (hotel == null)
            {
                return NotFound();
            }

            var checkInDate = checkIn ?? DateTime.Today.AddDays(1);
            var checkOutDate = checkOut ?? DateTime.Today.AddDays(2);
            if (checkOutDate <= checkInDate)
            {
                checkOutDate = checkInDate.AddDays(1);
            }

            bool isFavorite = false;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    isFavorite = await _context.Favorites.AnyAsync(f => f.HotelId == id && f.UserId == userId);
                }
            }

            var viewModel = new HotelDetailViewModel
            {
                Hotel = hotel,
                AvailableRooms = hotel.Rooms.Where(r => r.IsActive).ToList(),
                Reviews = hotel.Reviews.Where(r => r.Status == "Approved").OrderByDescending(r => r.CreatedAt).ToList(),
                IsFavorite = isFavorite,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                Guests = guests,
                Rooms = rooms
            };

            return View(viewModel);
        }

        // POST: /Hotel/AddReview
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int hotelId, int rating, string? title, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập nội dung đánh giá.";
                return RedirectToAction("Detail", new { id = hotelId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var review = new Review
            {
                HotelId = hotelId,
                UserId = user.Id,
                CustomerName = user.FullName,
                Rating = Math.Clamp(rating, 1, 5),
                Title = title ?? "Đánh giá kỳ nghỉ",
                Comment = comment,
                Status = "Approved",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá! Đánh giá của bạn đã được đăng thành công.";
            return RedirectToAction("Detail", new { id = hotelId });
        }

        // POST: /Hotel/ToggleFavorite
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int hotelId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để lưu khách sạn yêu thích." });
            }

            var existing = await _context.Favorites.FirstOrDefaultAsync(f => f.HotelId == hotelId && f.UserId == userId);
            bool isFavNow = false;

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                isFavNow = false;
            }
            else
            {
                var fav = new Favorite
                {
                    HotelId = hotelId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Favorites.AddAsync(fav);
                isFavNow = true;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, isFavorite = isFavNow, message = isFavNow ? "Đã thêm vào danh sách yêu thích!" : "Đã bỏ khỏi danh sách yêu thích!" });
        }

        // GET: /Hotel/Favorites
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var favorites = await _context.Favorites
                .Include(f => f.Hotel)
                    .ThenInclude(h => h!.Rooms)
                .Include(f => f.Hotel)
                    .ThenInclude(h => h!.Reviews)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Hotel!)
                .ToListAsync();

            return View(favorites);
        }
    }
}
