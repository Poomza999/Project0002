using Apartment.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apartment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Dormitory> Dormitories { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<DormRoomType> DormRoomTypes { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<DormAmenity> DormAmenities { get; set; }
        public DbSet<DormHistory> DormHistories { get; set; }
        public DbSet<DormImage> DormImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed RoomTypes แยกหมวดหมู่
            modelBuilder.Entity<RoomType>().HasData(
                // หมวดเตียง
                new RoomType { RoomTypeID = 1, Name = "เตียงเดี่ยว", Category = "ประเภทเตียง" },
                new RoomType { RoomTypeID = 2, Name = "เตียงคู่", Category = "ประเภทเตียง" },
                // หมวดสัญญา
                new RoomType { RoomTypeID = 3, Name = "รายวัน", Category = "ประเภทสัญญา" },
                new RoomType { RoomTypeID = 4, Name = "รายเดือน", Category = "ประเภทสัญญา" }
            );

            // Seed Amenities แยกหมวดหมู่
            modelBuilder.Entity<Amenity>().HasData(
                // หมวดในห้อง
                new Amenity { AmenityID = 1, Name = "แอร์", Category = "ในห้อง" },
                new Amenity { AmenityID = 2, Name = "ทีวี", Category = "ในห้อง" },
                new Amenity { AmenityID = 3, Name = "ตู้เย็น", Category = "ในห้อง" },
                new Amenity { AmenityID = 4, Name = "เฟอร์นิเจอร์พร้อมอยู่", Category = "ในห้อง" },
                // หมวดอินเตอร์เน็ต
                new Amenity { AmenityID = 5, Name = "อินเตอร์เน็ตฟรี", Category = "อินเตอร์เน็ต" },
                new Amenity { AmenityID = 6, Name = "อินเตอร์เน็ตมีค่าบริการ", Category = "อินเตอร์เน็ต" },
                // หมวดซักผ้า
                new Amenity { AmenityID = 7, Name = "เครื่องซักผ้าภายในหอพัก", Category = "ซักผ้า" },
                // หมวดความปลอดภัย
                new Amenity { AmenityID = 8, Name = "กล้องวงจรปิด", Category = "ความปลอดภัย" },
                new Amenity { AmenityID = 9, Name = "คีย์การ์ด / รหัสเข้าห้อง", Category = "ความปลอดภัย" },
                // หมวดส่วนกลาง
                new Amenity { AmenityID = 10, Name = "ที่จอดรถ", Category = "ส่วนกลาง" },
                new Amenity { AmenityID = 11, Name = "ฟิตเนส", Category = "ส่วนกลาง" },
                new Amenity { AmenityID = 12, Name = "สระว่ายน้ำ", Category = "ส่วนกลาง" }
            );

            // Seed Admin
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    Name = "Admin",
                    Email = "admin@npru.ac.th",
                    PasswordHash = "$2a$11$4G1x5/N3B.WHHDrGG6a4vuiH.B7LixQYFppacceYsbso81qJIu8WG",
                    Role = "Admin"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}