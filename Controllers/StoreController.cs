using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using WaterPlantApp.Data;
using WaterPlantApp.Models;

namespace WaterPlantApp.Controllers
{
    public class StoreController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public StoreController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // PUBLIC: /Store
        public async Task<IActionResult> Index(string? search, string? city, string? type)
        {
            var query = _db.Stores
                .Include(s => s.StoreProducts).ThenInclude(sp => sp.Product)
                .Where(s => s.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.StoreName.Contains(search) || s.City.Contains(search) || s.StoreCode.Contains(search));
            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(s => s.City == city);
            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(s => s.StoreType == type);

            var stores = await query.OrderBy(s => s.StoreId).ToListAsync();
            ViewBag.Cities      = await _db.Stores.Where(s => s.IsActive).Select(s => s.City).Distinct().ToListAsync();
            ViewBag.Search      = search;
            ViewBag.City        = city;
            ViewBag.Type        = type;
            ViewBag.TotalStores = stores.Count;
            return View(stores);
        }

        // PUBLIC: /Store/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var store = await _db.Stores
                .Include(s => s.StoreProducts).ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(s => s.StoreId == id && s.IsActive);
            if (store == null) return NotFound();
            return View(store);
        }

        // ADMIN: /Store/Manage
        public async Task<IActionResult> Manage()
        {
            var stores = await _db.Stores.Include(s => s.StoreProducts)
                .OrderByDescending(s => s.CreatedAt).ToListAsync();
            return View(stores);
        }

        // ADMIN: Create
        public IActionResult Create() => View(new Store());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Store store)
        {
            if (!ModelState.IsValid) return View(store);
            store.CreatedAt = store.UpdatedAt = DateTime.UtcNow;
            _db.Stores.Add(store);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Store '{store.StoreName}' created successfully!";
            return RedirectToAction(nameof(Manage));
        }

        // ADMIN: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var store = await _db.Stores.FindAsync(id);
            if (store == null) return NotFound();
            return View(store);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Store store)
        {
            if (id != store.StoreId) return BadRequest();
            if (!ModelState.IsValid) return View(store);
            store.UpdatedAt = DateTime.UtcNow;
            _db.Update(store);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Store updated successfully!";
            return RedirectToAction(nameof(Manage));
        }

        // ADMIN: Delete (soft)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var store = await _db.Stores.FindAsync(id);
            if (store == null) return NotFound();
            store.IsActive = false;
            store.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Store deactivated successfully.";
            return RedirectToAction(nameof(Manage));
        }

        // QR: /Store/QRCode  returns PNG
        [HttpGet]
        public IActionResult QRCode(string? url)
        {
            var baseUrl = _config["AppSettings:BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            var targetUrl = url ?? $"{baseUrl}/Store";

            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(targetUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var pngBytes = qrCode.GetGraphic(20,
                darkColorRgba:  new byte[] { 0, 61, 122, 255 },
                lightColorRgba: new byte[] { 255, 255, 255, 255 });
            return File(pngBytes, "image/png");
        }

        // QR: /Store/QRPage  printable page
        public IActionResult QRPage()
        {
            var baseUrl = _config["AppSettings:BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            ViewBag.StoreListUrl = $"{baseUrl}/Store";
            return View();
        }
    }
}
