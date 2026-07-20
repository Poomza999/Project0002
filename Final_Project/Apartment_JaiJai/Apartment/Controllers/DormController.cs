using Apartment.Data;
using Apartment.Models.Entities;
using Apartment.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Apartment.Controllers
{
    public class DormController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public DormController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private bool IsOwnerLoggedIn(out int userID)
        {
            var idStr = HttpContext.Session.GetString("UserID");
            var role = HttpContext.Session.GetString("UserRole");
            userID = 0;
            if (idStr == null || role != "Owner") return false;
            userID = int.Parse(idStr);
            return true;
        }

        public IActionResult Index()
        {
            if (!IsOwnerLoggedIn(out int ownerID))
                return RedirectToAction("Login", "Account");

            var dorms = _db.Dormitories
                .Include(d => d.DormRoomTypes).ThenInclude(rt => rt.RoomType)
                .Include(d => d.Images)
                .Where(d => d.OwnerID == ownerID)
                .OrderByDescending(d => d.UpdatedAt)
                .ToList();

            return View(dorms);
        }

        public IActionResult Create()
        {
            if (!IsOwnerLoggedIn(out _))
                return RedirectToAction("Login", "Account");

            return View(BuildFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult DeleteImage(int imageID, int dormID)
        {
            if (!IsOwnerLoggedIn(out int ownerID))
                return Json(new { success = false, message = "กรุณาเข้าสู่ระบบ" });

            var dorm = _db.Dormitories
                .FirstOrDefault(d => d.DormID == dormID && d.OwnerID == ownerID);

            if (dorm == null)
                return Json(new { success = false, message = "ไม่พบหอพัก" });

            var image = _db.DormImages
                .FirstOrDefault(i => i.ImageID == imageID && i.DormID == dormID);

            if (image == null)
                return Json(new { success = false, message = "ไม่พบรูปภาพ" });

            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, image.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                _db.DormImages.Remove(image);
                _db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "เกิดข้อผิดพลาด" });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DormFormViewModel model)
        {
            if (!IsOwnerLoggedIn(out int ownerID))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            var dorm = new Dormitory
            {
                OwnerID = ownerID,
                DormName = model.DormName,
                Address = model.Address,
                Price = model.Price,
                Phone = model.Phone,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.Dormitories.Add(dorm);
            _db.SaveChanges();

            // บันทึก RoomTypes
            foreach (var rtID in model.SelectedRoomTypeIDs)
            {
                _db.DormRoomTypes.Add(new DormRoomType
                {
                    DormID = dorm.DormID,
                    RoomTypeID = rtID
                });
            }

            // บันทึกรูปหลายรูป
            if (model.ImageFiles != null && model.ImageFiles.Count > 0)
            {
                int order = 0;
                foreach (var file in model.ImageFiles)
                {
                    var path = await SaveImage(file);
                    if (path != null)
                    {
                        _db.DormImages.Add(new DormImage
                        {
                            DormID = dorm.DormID,
                            ImagePath = path,
                            SortOrder = order++
                        });
                    }
                }
            }

            // บันทึก Amenities
            foreach (var aid in model.SelectedAmenityIDs)
            {
                _db.DormAmenities.Add(new DormAmenity
                {
                    DormID = dorm.DormID,
                    AmenityID = aid
                });
            }

            _db.DormHistories.Add(new DormHistory
            {
                DormID = dorm.DormID,
                Action = "Submitted",
                Note = "ส่งข้อมูลเพื่อรอการอนุมัติ",
                ChangedAt = DateTime.Now
            });

            _db.SaveChanges();

            TempData["Success"] = "เพิ่มหอพักสำเร็จ กรุณารอการอนุมัติจากมหาวิทยาลัย";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!IsOwnerLoggedIn(out int ownerID))
                return RedirectToAction("Login", "Account");

            var dorm = _db.Dormitories
                .Include(d => d.DormRoomTypes)
                .Include(d => d.DormAmenities)
                .Include(d => d.Images)
                .FirstOrDefault(d => d.DormID == id && d.OwnerID == ownerID);

            if (dorm == null) return NotFound();

            if (dorm.Status == "Pending" || dorm.Status == "PendingEdit")
            {
                TempData["Error"] = "ไม่สามารถแก้ไขได้ขณะรอการอนุมัติ";
                return RedirectToAction("Index");
            }

            var vm = BuildFormViewModel();
            vm.DormID = dorm.DormID;
            vm.DormName = dorm.DormName;
            vm.Address = dorm.Address;
            vm.Price = dorm.Price;
            vm.Phone = dorm.Phone;
            vm.ExistingImages = dorm.Images
                .OrderBy(i => i.SortOrder)
                .ToList();
            vm.SelectedRoomTypeIDs = dorm.DormRoomTypes
                .Select(r => r.RoomTypeID).ToList();
            vm.SelectedAmenityIDs = dorm.DormAmenities
                .Select(a => a.AmenityID).ToList();

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DormFormViewModel model)
        {
            if (!IsOwnerLoggedIn(out int ownerID))
                return RedirectToAction("Login", "Account");

            var dorm = _db.Dormitories
                .Include(d => d.DormRoomTypes)
                .Include(d => d.DormAmenities)
                .Include(d => d.Images)
                .FirstOrDefault(d => d.DormID == id && d.OwnerID == ownerID);

            if (dorm == null) return NotFound();

            if (!ModelState.IsValid)
                return View(BuildFormViewModel(model));

            dorm.DormName = model.DormName;
            dorm.Address = model.Address;
            dorm.Price = model.Price;
            dorm.Phone = model.Phone;
            dorm.Status = "PendingEdit";
            dorm.UpdatedAt = DateTime.Now;

            // อัปเดต RoomTypes
            _db.DormRoomTypes.RemoveRange(dorm.DormRoomTypes);
            foreach (var rtID in model.SelectedRoomTypeIDs)
            {
                _db.DormRoomTypes.Add(new DormRoomType
                {
                    DormID = dorm.DormID,
                    RoomTypeID = rtID
                });
            }

            // เพิ่มรูปใหม่
            if (model.ImageFiles != null && model.ImageFiles.Count > 0)
            {
                int order = dorm.Images.Count;
                foreach (var file in model.ImageFiles)
                {
                    var path = await SaveImage(file);
                    if (path != null)
                    {
                        _db.DormImages.Add(new DormImage
                        {
                            DormID = dorm.DormID,
                            ImagePath = path,
                            SortOrder = order++
                        });
                    }
                }
            }

            // อัปเดต Amenities
            _db.DormAmenities.RemoveRange(dorm.DormAmenities);
            foreach (var aid in model.SelectedAmenityIDs)
            {
                _db.DormAmenities.Add(new DormAmenity
                {
                    DormID = dorm.DormID,
                    AmenityID = aid
                });
            }

            _db.DormHistories.Add(new DormHistory
            {
                DormID = dorm.DormID,
                Action = "Edited",
                Note = "เจ้าของแก้ไขข้อมูลและส่งรออนุมัติอีกครั้ง",
                ChangedAt = DateTime.Now
            });

            _db.SaveChanges();

            TempData["Success"] = "แก้ไขข้อมูลสำเร็จ กรุณารอการอนุมัติจากมหาวิทยาลัย";
            return RedirectToAction("Index");
        }

        private async Task<string?> SaveImage(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }

        private DormFormViewModel BuildFormViewModel(DormFormViewModel? existing = null)
        {
            var vm = existing ?? new DormFormViewModel();
            vm.RoomTypes = _db.RoomTypes
                .GroupBy(r => r.Category)
                .ToDictionary(g => g.Key, g => g.Select(r => new SelectListItem
                {
                    Value = r.RoomTypeID.ToString(),
                    Text = r.Name
                }).ToList());
            vm.AllAmenities = _db.Amenities
                .GroupBy(a => a.Category)
                .ToDictionary(g => g.Key, g => g.Select(a => new SelectListItem
                {
                    Value = a.AmenityID.ToString(),
                    Text = a.Name
                }).ToList());
            return vm;
        }
    }
}