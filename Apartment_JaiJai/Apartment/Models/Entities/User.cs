using System.ComponentModel.DataAnnotations;

    namespace Apartment.Models.Entities
    {
        public class User
        {
            public int UserID { get; set; }

            [Required, MaxLength(100)]
            public string Name { get; set; } = string.Empty;

            [Required, MaxLength(150)]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string PasswordHash { get; set; } = string.Empty;

            // "Owner" หรือ "Admin"
            public string Role { get; set; } = "Owner";

            public ICollection<Dormitory> Dormitories { get; set; } = new List<Dormitory>();
        }
    }
