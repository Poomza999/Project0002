using Apartment.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Apartment.Controllers
{
    public class ChatController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public ChatController(AppDbContext db, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        // GET: /Chat
        public IActionResult Index()
        {
            var apiKey = _config["GeminiApiKey"];
            ViewBag.HasApiKey = !string.IsNullOrEmpty(apiKey) ? "true" : "";
            return View();
        }

        // POST: /Chat/Send
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return Json(new { reply = "กรุณาพิมพ์คำถามครับ" });

            // ดึงข้อมูลหอพักที่ Approved จาก Database
            var dorms = await _db.Dormitories
                .Include(d => d.DormRoomTypes).ThenInclude(r => r.RoomType)
                .Include(d => d.DormAmenities).ThenInclude(a => a.Amenity)
                .Where(d => d.Status == "Approved")
                .ToListAsync();

            // สร้าง Context ข้อมูลหอพักให้ Bot
            var dormInfo = new StringBuilder();
            dormInfo.AppendLine("ข้อมูลหอพักที่ผ่านการรับรองจากมหาวิทยาลัยราชภัฏนครปฐม:");
            dormInfo.AppendLine();

            if (dorms.Any())
            {
                foreach (var dorm in dorms)
                {
                    dormInfo.AppendLine($"- ชื่อ: {dorm.DormName}");
                    dormInfo.AppendLine($"  ที่อยู่: {dorm.Address}");
                    dormInfo.AppendLine($"  ราคา: {dorm.Price:N0} บาท/เดือน");
                    dormInfo.AppendLine($"  เบอร์ติดต่อ: {dorm.Phone ?? "ไม่มีข้อมูล"}");
                    dormInfo.AppendLine($"  ประเภทห้อง: {string.Join(", ", dorm.DormRoomTypes.Select(r => r.RoomType.Name))}");
                    dormInfo.AppendLine($"  สิ่งอำนวยความสะดวก: {string.Join(", ", dorm.DormAmenities.Select(a => a.Amenity.Name))}");
                    dormInfo.AppendLine();
                }
            }
            else
            {
                dormInfo.AppendLine("ยังไม่มีหอพักที่ผ่านการรับรองในขณะนี้");
            }

            var systemPrompt = $@"คุณคือ DormBot ผู้ช่วยค้นหาหอพักสำหรับนักศึกษามหาวิทยาลัยราชภัฏนครปฐม
                                ตอบเป็นภาษาไทยเท่านั้น ตอบสั้นๆ กระชับ เป็นมิตร
                                ห้ามแต่งข้อมูลที่ไม่มีอยู่จริง ถ้าไม่มีข้อมูลให้บอกตรงๆ
                                ถ้าถามเรื่องอื่นที่ไม่เกี่ยวกับหอพักให้บอกว่าตอบได้เฉพาะเรื่องหอพักเท่านั้น
            {dormInfo}";

            try
            {
                var apiKey = _config["AnthropicApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                    return Json(new { reply = "ขออภัยครับ ระบบ Chatbot ยังไม่พร้อมใช้งาน" });

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var body = new
                {
                    model = "claude-haiku-4-5",
                    max_tokens = 500,
                    system = systemPrompt,
                    messages = new[]
                    {
            new { role = "user", content = request.Message }
        }
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.anthropic.com/v1/messages", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return Json(new { reply = $"API Error: {response.StatusCode} - {responseJson}" });

                var doc = JsonDocument.Parse(responseJson);
                var reply = doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString();

                return Json(new { reply });
            }
            catch (Exception ex)
            {
                return Json(new { reply = $"Error: {ex.Message}" });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}