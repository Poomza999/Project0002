using Apartment.Data;
using Apartment.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apartment.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult Index(string? filter = null)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            var query = _db.Dormitories
                .Include(d => d.DormRoomTypes).ThenInclude(rt => rt.RoomType)
                .Include(d => d.Owner)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
                query = query.Where(d => d.Status == filter);

            ViewBag.CurrentFilter = filter;
            return View(query.OrderByDescending(d => d.UpdatedAt).ToList());
        }

        public IActionResult Login()
        {
            if (IsAdminLoggedIn())
                return RedirectToAction("Index", "Admin");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email && u.Role == "Admin");

            if (user == null)
            {
                ViewBag.Error = "ไม่พบ Email นี้ในระบบ";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "รหัสผ่านไม่ถูกต้อง";
                return View();
            }

            HttpContext.Session.SetString("UserID", user.UserID.ToString());
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Detail(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            var dorm = _db.Dormitories
                .Include(d => d.DormRoomTypes).ThenInclude(rt => rt.RoomType)
                .Include(d => d.Owner)
                .Include(d => d.DormAmenities).ThenInclude(da => da.Amenity)
                .Include(d => d.Images)
                .Include(d => d.Histories)
                .FirstOrDefault(d => d.DormID == id);

            if (dorm == null) return NotFound();
            return View(dorm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            var dorm = _db.Dormitories.Find(id);
            if (dorm == null) return NotFound();

            dorm.Status = "Approved";
            dorm.RejectReason = null;
            dorm.UpdatedAt = DateTime.Now;

            _db.DormHistories.Add(new DormHistory
            {
                DormID = dorm.DormID,
                Action = "Approved",
                Note = "Admin อนุมัติแล้ว",
                ChangedAt = DateTime.Now
            });

            _db.SaveChanges();
            TempData["Success"] = $"อนุมัติ '{dorm.DormName}' เรียบร้อยแล้ว";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string reason)
        {
            if (!IsAdminLoggedIn())
                return RedirectToAction("Login", "Admin");

            var dorm = _db.Dormitories.Find(id);
            if (dorm == null) return NotFound();

            dorm.Status = "Rejected";
            dorm.RejectReason = reason;
            dorm.UpdatedAt = DateTime.Now;

            _db.DormHistories.Add(new DormHistory
            {
                DormID = dorm.DormID,
                Action = "Rejected",
                Note = $"Admin ปฏิเสธ: {reason}",
                ChangedAt = DateTime.Now
            });

            _db.SaveChanges();
            TempData["Error"] = $"ปฏิเสธ '{dorm.DormName}' เรียบร้อยแล้ว";
            return RedirectToAction("Index");
        }
    }
}