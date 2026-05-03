using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterPlantApp.Data;

namespace WaterPlantApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStores   = await _db.Stores.CountAsync(s => s.IsActive);
            ViewBag.TotalProducts = await _db.Products.CountAsync(p => p.IsActive);
            return View();
        }

        public IActionResult Error() => View();
    }
}
