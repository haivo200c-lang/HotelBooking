using HotelBooking.Models.Entities;

namespace HotelBooking.Models.ViewModels
{
    public class HotelSearchFilterViewModel
    {
        public string? Destination { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int Guests { get; set; } = 2;
        public int Rooms { get; set; } = 1;

        // Filter options
        public string? City { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? StarRating { get; set; }
        public double? MinRating { get; set; }
        public List<int> SelectedAmenityIds { get; set; } = new List<int>();

        // Sorting option
        public string SortBy { get; set; } = "featured"; // featured, price_asc, price_desc, rating_desc

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalHotels { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalHotels / PageSize);

        // Results
        public List<Hotel> Hotels { get; set; } = new List<Hotel>();
        public List<Amenity> AvailableAmenities { get; set; } = new List<Amenity>();
        public List<string> AvailableCities { get; set; } = new List<string>();
    }

    public class HotelDetailViewModel
    {
        public Hotel Hotel { get; set; } = null!;
        public List<Room> AvailableRooms { get; set; } = new List<Room>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public bool IsFavorite { get; set; }

        // Search parameters passed from query
        public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);
        public int Guests { get; set; } = 2;
        public int Rooms { get; set; } = 1;
    }

    public class RoomDetailViewModel
    {
        public Room Room { get; set; } = null!;
        public Hotel Hotel { get; set; } = null!;
        public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);
        public int Guests { get; set; } = 2;
        public int Quantity { get; set; } = 1;

        public int TotalNights => Math.Max(1, (CheckOutDate.Date - CheckInDate.Date).Days);
        public decimal SubTotal => Room.PricePerNight * TotalNights * Quantity;
        public decimal TaxAmount => Math.Round(SubTotal * 0.08m, 0);
        public decimal ServiceFee => Math.Round(SubTotal * 0.05m, 0);
        public decimal TotalAmount => SubTotal + TaxAmount + ServiceFee;
    }

    public class HomeViewModel
    {
        public List<Hotel> FeaturedHotels { get; set; } = new List<Hotel>();
        public List<Hotel> TopRatedHotels { get; set; } = new List<Hotel>();
        public List<DestinationViewModel> PopularDestinations { get; set; } = new List<DestinationViewModel>();
        public List<Review> RecentReviews { get; set; } = new List<Review>();
        public int TotalHotelsCount { get; set; }
        public int TotalRoomsCount { get; set; }
    }

    public class DestinationViewModel
    {
        public string CityName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int HotelCount { get; set; }
        public decimal StartingPrice { get; set; }
    }
}
