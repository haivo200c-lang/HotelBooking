using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models.Entities
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
