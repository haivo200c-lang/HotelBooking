using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models.Entities
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Icon { get; set; } = "bi-check-circle"; // Bootstrap icon class or fa icon

        [StringLength(50)]
        public string Category { get; set; } = "General"; // General, Room, Service, Wellness

        public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
        public virtual ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}
