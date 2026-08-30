using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }

    public class AppRole : IdentityRole
    {
        public string? Description { get; set; }

        public AppRole() : base() { }
        public AppRole(string roleName) : base(roleName) { }
    }
}
