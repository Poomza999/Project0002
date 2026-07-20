using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apartment.Models.Entities
{
    public class Dormitory
    {
        [Key]
        public int DormID { get; set; }

        public int OwnerID { get; set; }

        [Required, MaxLength(150)]
        public string DormName { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public string? ImagePath { get; set; }

        public string Status { get; set; } = "Pending";

        public string? Phone { get; set; }

        public string? RejectReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [ForeignKey("OwnerID")]
        public User Owner { get; set; } = null!;

        public ICollection<DormRoomType> DormRoomTypes { get; set; } = new List<DormRoomType>();
        public ICollection<DormAmenity> DormAmenities { get; set; } = new List<DormAmenity>();
        public ICollection<DormHistory> Histories { get; set; } = new List<DormHistory>();
        public ICollection<DormImage> Images { get; set; } = new List<DormImage>();
    }
}