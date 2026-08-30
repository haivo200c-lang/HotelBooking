using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models.Entities
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Slug { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = "Việt Nam";

        [Range(1, 5)]
        public int StarRating { get; set; } = 5;

        [StringLength(50)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(500)]
        public string MainImageUrl { get; set; } = string.Empty;

        public bool IsFeatured { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();
        public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // Computed helper properties
        [NotMapped]
        public double AverageRating => Reviews.Any(r => r.Status == "Approved") 
            ? Math.Round(Reviews.Where(r => r.Status == "Approved").Average(r => r.Rating), 1) 
            : 4.8;

        [NotMapped]
        public int TotalReviewCount => Reviews.Count(r => r.Status == "Approved");

        [NotMapped]
        public decimal MinPrice => Rooms.Any() ? Rooms.Min(r => r.PricePerNight) : 0;
    }
}
