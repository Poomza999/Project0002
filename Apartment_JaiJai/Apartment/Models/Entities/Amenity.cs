using System.ComponentModel.DataAnnotations;

namespace Apartment.Models.Entities
{
    public class Amenity
    {
        [Key]
        public int AmenityID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public ICollection<DormAmenity> DormAmenities { get; set; } = new List<DormAmenity>();
    }
}