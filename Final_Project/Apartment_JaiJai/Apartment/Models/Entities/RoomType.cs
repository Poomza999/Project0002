using System.ComponentModel.DataAnnotations;

namespace Apartment.Models.Entities
{
    public class RoomType
    {
        [Key]
        public int RoomTypeID { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // หมวดหมู่ เช่น "เตียง" "สัญญา"
        public string Category { get; set; } = string.Empty;

        public ICollection<DormRoomType> DormRoomTypes { get; set; } = new List<DormRoomType>();
    }
}