using System.ComponentModel.DataAnnotations.Schema;

namespace Apartment.Models.Entities
{
    public class DormAmenity
    {
        public int DormAmenityID { get; set; }

        public int DormID { get; set; }

        public int AmenityID { get; set; }

        [ForeignKey("DormID")]
        public Dormitory Dormitory { get; set; } = null!;

        [ForeignKey("AmenityID")]
        public Amenity Amenity { get; set; } = null!;
    }
}