using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        public virtual Hotel? Hotel { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [ForeignKey("RoomTypeId")]
        public virtual RoomType? RoomType { get; set; }

        [StringLength(50)]
        public string? RoomNumber { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100000000)]
        public decimal PricePerNight { get; set; }

        [Range(1, 20)]
        public int Capacity { get; set; } = 2;

        [StringLength(100)]
        public string BedInfo { get; set; } = "1 Giường King Size";

        public double AreaM2 { get; set; } = 35.0;

        [Range(1, 1000)]
        public int TotalQuantity { get; set; } = 10;

        [Range(0, 1000)]
        public int AvailableQuantity { get; set; } = 10;

        public bool IsActive { get; set; } = true;

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
        public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    }

    public class RoomImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Caption { get; set; }

        public bool IsPrimary { get; set; } = false;
    }

    public class RoomAmenity
    {
        public int RoomId { get; set; }
        public virtual Room? Room { get; set; }

        public int AmenityId { get; set; }
        public virtual Amenity? Amenity { get; set; }
    }
}
