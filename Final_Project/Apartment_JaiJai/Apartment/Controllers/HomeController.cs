using Apartment.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apartment.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /
        public IActionResult Index(string? keyword, int? roomTypeID)
        {
            var query = _db.Dormitories
                .Include(d => d.DormRoomTypes).ThenInclude(rt => rt.RoomType)
                .Include(d => d.Images)
                .Where(d => d.Status == "Approved")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(d =>
                    d.DormName.ToLower().Contains(keyword) ||
                    d.Address.ToLower().Contains(keyword));
            }

            if (roomTypeID.HasValue)
                query = query.Where(d =>
                    d.DormRoomTypes.Any(rt => rt.RoomTypeID == roomTypeID));

            ViewBag.Keyword = keyword;
            ViewBag.RoomTypeID = roomTypeID;
            ViewBag.RoomTypes = _db.RoomTypes.ToList();
            ViewBag.IsSearching = !string.IsNullOrWhiteSpace(keyword) || roomTypeID.HasValue;

            return View(query.OrderByDescending(d => d.UpdatedAt).ToList());
        }

        // GET: /Home/Detail/5
        public IActionResult Detail(int id)
        {
            var dorm = _db.Dormitories
                .Include(d => d.DormRoomTypes).ThenInclude(rt => rt.RoomType)
                .Include(d => d.DormAmenities).ThenInclude(da => da.Amenity)
                .Include(d => d.Images)
                .FirstOrDefault(d => d.DormID == id && d.Status == "Approved");

            if (dorm == null) return NotFound();
            return View(dorm);
        }
    }
}