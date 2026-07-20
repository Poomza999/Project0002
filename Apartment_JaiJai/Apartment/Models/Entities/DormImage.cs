using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apartment.Models.Entities
{
    public class DormImage
    {
        [Key]
        public int ImageID { get; set; }

        public int DormID { get; set; }

        public string ImagePath { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 0;

        [ForeignKey("DormID")]
        public Dormitory Dormitory { get; set; } = null!;
    }
}