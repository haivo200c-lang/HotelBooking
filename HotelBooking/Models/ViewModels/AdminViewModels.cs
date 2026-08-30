using HotelBooking.Models.Entities;

namespace HotelBooking.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalHotels { get; set; }
        public int TotalRooms { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingBookingsCount { get; set; }
        public int CompletedBookingsCount { get; set; }

        public List<Booking> RecentBookings { get; set; } = new List<Booking>();

        // Chart Data
        public List<string> Months { get; set; } = new List<string>();
        public List<decimal> MonthlyRevenues { get; set; } = new List<decimal>();
        public List<int> MonthlyBookingCounts { get; set; } = new List<int>();

        public Dictionary<string, int> StatusDistribution { get; set; } = new Dictionary<string, int>();
    }

    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Customer";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalBookingsCount { get; set; }
    }
}
