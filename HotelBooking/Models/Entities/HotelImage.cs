using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models.Entities
{
    public class HotelImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        public virtual Hotel? Hotel { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Caption { get; set; }

        public bool IsPrimary { get; set; } = false;
    }

    public class HotelAmenity
    {
        public int HotelId { get; set; }
        public virtual Hotel? Hotel { get; set; }

        public int AmenityId { get; set; }
        public virtual Amenity? Amenity { get; set; }
    }
}
