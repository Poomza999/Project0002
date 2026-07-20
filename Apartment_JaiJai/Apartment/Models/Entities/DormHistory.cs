using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apartment.Models.Entities
{
    public class DormHistory
    {
        [Key]
        public int HistoryID { get; set; }

        public int DormID { get; set; }

        public string Action { get; set; } = string.Empty;

        public string? Note { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.Now;

        [ForeignKey("DormID")]
        public Dormitory Dormitory { get; set; } = null!;
    }
}