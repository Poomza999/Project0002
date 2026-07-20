using Apartment.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Apartment.Models.ViewModels
{
    public class DormFormViewModel
    {
        public int DormID { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อหอพัก")]
        public string DormName { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกที่อยู่")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกราคา")]
        [Range(1, 999999, ErrorMessage = "ราคาไม่ถูกต้อง")]
        public decimal Price { get; set; }

        public string? Phone { get; set; }

        public List<IFormFile>? ImageFiles { get; set; }

        // เปลี่ยนจาก List<string> เป็น List<DormImage>
        public List<DormImage> ExistingImages { get; set; } = new();

        // ยังคงไว้เพื่อ Backward Compatible
        public List<string> ExistingImagePaths => ExistingImages.Select(i => i.ImagePath).ToList();

        public List<int> SelectedRoomTypeIDs { get; set; } = new();
        public List<int> SelectedAmenityIDs { get; set; } = new();

        public Dictionary<string, List<SelectListItem>> RoomTypes { get; set; } = new();
        public Dictionary<string, List<SelectListItem>> AllAmenities { get; set; } = new();
    }
}