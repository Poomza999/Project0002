using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apartment.Models.Entities
{
    public class DormRoomType
    {
        [Key]
        public int DormRoomTypeID { get; set; }

        public int DormID { get; set; }

        public int RoomTypeID { get; set; }

        [ForeignKey("DormID")]
        public Dormitory Dormitory { get; set; } = null!;

        [ForeignKey("RoomTypeID")]
        public RoomType RoomType { get; set; } = null!;
    }
}