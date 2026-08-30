using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Models.ViewModels;

namespace HotelBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var featuredHotels = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Reviews)
                .Include(h => h.HotelAmenities)
                    .ThenInclude(ha => ha.Amenity)
                .Where(h => h.IsActive && h.IsFeatured)
                .Take(6)
                .ToListAsync();

            var topRatedHotels = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Reviews)
                .Include(h => h.HotelAmenities)
                    .ThenInclude(ha => ha.Amenity)
                .Where(h => h.IsActive)
                .Take(6)
                .ToListAsync();

            var popularDestinations = new List<DestinationViewModel>
            {
                new() { CityName = "Đà Nẵng", ImageUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&w=600&q=80", HotelCount = await _context.Hotels.CountAsync(h => h.City == "Đà Nẵng"), StartingPrice = 3200000 },
                new() { CityName = "Phú Quốc", ImageUrl = "https://images.unsplash.com/photo-1589779267156-f40b25e7960c?auto=format&fit=crop&w=600&q=80", HotelCount = await _context.Hotels.CountAsync(h => h.City == "Phú Quốc"), StartingPrice = 6800000 },
                new() { CityName = "TP. Hồ Chí Minh", ImageUrl = "https://images.unsplash.com/photo-1583417319070-4a69db38a482?auto=format&fit=crop&w=600&q=80", HotelCount = await _context.Hotels.CountAsync(h => h.City == "TP. Hồ Chí Minh"), StartingPrice = 4500000 },
                new() { CityName = "Hà Nội", ImageUrl = "https://images.unsplash.com/photo-1509042239860-f550ce710b93?auto=format&fit=crop&w=600&q=80", HotelCount = await _context.Hotels.CountAsync(h => h.City == "Hà Nội"), StartingPrice = 7200000 },
                new() { CityName = "Nha Trang", ImageUrl = "https://images.unsplash.com/photo-1570789210967-2cac24afeb00?auto=format&fit=crop&w=600&q=80", HotelCount = await _context.Hotels.CountAsync(h => h.City == "Nha Trang"), StartingPrice = 2600000 },
                new() { CityName = "Sapa", ImageUrl = "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?auto=format&fit=crop&w=600&q=80", HotelCount = await _context.Hotels.CountAsync(h => h.City == "Sapa"), StartingPrice = 3800000 }
            };

            var recentReviews = await _context.Reviews
                .Include(r => r.Hotel)
                .Where(r => r.Status == "Approved")
                .OrderByDescending(r => r.CreatedAt)
                .Take(4)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedHotels = featuredHotels,
                TopRatedHotels = topRatedHotels,
                PopularDestinations = popularDestinations,
                RecentReviews = recentReviews,
                TotalHotelsCount = await _context.Hotels.CountAsync(h => h.IsActive),
                TotalRoomsCount = await _context.Rooms.CountAsync(r => r.IsActive)
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Deals()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
