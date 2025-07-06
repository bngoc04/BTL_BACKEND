using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFood.Data;
using WebFood.Models;

namespace WebFood.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách danh mục
            var danhMucs = await _context.DanhMucs.ToListAsync();
            ViewBag.DanhMucs = danhMucs;

            // Lấy sản phẩm mới nhất
            var sanPhamMoiNhat = await _context.SanPhams
                .OrderByDescending(s => s.Id)
                .Take(8)
                .ToListAsync();
            ViewBag.SanPhamMoiNhat = sanPhamMoiNhat;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
