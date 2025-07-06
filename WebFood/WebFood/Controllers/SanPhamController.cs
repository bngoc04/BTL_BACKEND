using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFood.Data;
using WebFood.Models;
using WebFood.Models.ViewModels;

namespace WebFood.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? danhMucId, int? datBanId)
        {
            var sanPhams = _context.SanPhams
                .Include(s => s.DanhMuc)
                .AsQueryable();

            if (danhMucId.HasValue)
            {
                sanPhams = sanPhams.Where(s => s.DanhMucId == danhMucId);
                ViewBag.DanhMucHienTai = await _context.DanhMucs.FindAsync(danhMucId);
            }

            // Nếu có datBanId, lưu vào ViewBag để sử dụng trong view
            if (datBanId.HasValue)
            {
                var datBan = await _context.DatBans
                    .Include(d => d.BanAn)
                    .FirstOrDefaultAsync(d => d.DatBanId == datBanId);
                
                if (datBan != null)
                {
                    ViewBag.DatBan = datBan;
                    // Lưu datBanId vào session để sử dụng khi thanh toán
                    HttpContext.Session.SetInt32("DatBanId", datBanId.Value);
                }
            }

            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
            return View(await sanPhams.ToListAsync());
        }

        public async Task<IActionResult> ChiTiet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        [HttpPost]
        public IActionResult ThemVaoGioHang(int sanPhamId, int soLuong, bool stayOnPage = false, int? danhMucId = null, int? datBanId = null)
        {
            var sanPham = _context.SanPhams.Find(sanPhamId);
            if (sanPham == null)
            {
                return NotFound();
            }

            var gioHang = LayGioHang();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var sanPhamTrongGio = gioHang.FirstOrDefault(i => i.SanPhamId == sanPhamId);
            if (sanPhamTrongGio != null)
            {
                // Nếu đã có, tăng số lượng
                sanPhamTrongGio.SoLuong += soLuong;
            }
            else
            {
                // Nếu chưa có, thêm mới vào giỏ hàng
                gioHang.Add(new GioHangItem
                {
                    SanPhamId = sanPham.Id,
                    TenSanPham = sanPham.Ten,
                    DonGia = sanPham.Gia,
                    HinhAnh = sanPham.HinhAnh,
                    SoLuong = soLuong
                });
            }

            LuuGioHang(gioHang);
            
            // Hiển thị thông báo thành công
            TempData["Success"] = $"Đã thêm {soLuong} {sanPham.Ten} vào giỏ hàng!";
            
            // Nếu stayOnPage = true, ở lại trang thực đơn
            if (stayOnPage)
            {
                return RedirectToAction("Index", new { danhMucId = danhMucId, datBanId = datBanId });
            }
            else
            {
                return RedirectToAction("GioHang");
            }
        }

        public IActionResult GioHang()
        {
            var gioHang = LayGioHang();
            
            // Kiểm tra xem có đang đặt món cho bàn không
            var datBanId = HttpContext.Session.GetInt32("DatBanId");
            if (datBanId.HasValue)
            {
                ViewBag.DatBanId = datBanId.Value;
            }
            
            return View(gioHang);
        }

        [HttpPost]
        public IActionResult CapNhatGioHang(int sanPhamId, int soLuong)
        {
            var gioHang = LayGioHang();
            var sanPhamTrongGio = gioHang.FirstOrDefault(i => i.SanPhamId == sanPhamId);

            if (sanPhamTrongGio != null)
            {
                if (soLuong > 0)
                {
                    sanPhamTrongGio.SoLuong = soLuong;
                }
                else
                {
                    gioHang.Remove(sanPhamTrongGio);
                }
            }

            LuuGioHang(gioHang);
            return RedirectToAction("GioHang");
        }

        [HttpPost]
        public IActionResult XoaKhoiGioHang(int sanPhamId)
        {
            var gioHang = LayGioHang();
            var sanPhamTrongGio = gioHang.FirstOrDefault(i => i.SanPhamId == sanPhamId);

            if (sanPhamTrongGio != null)
            {
                gioHang.Remove(sanPhamTrongGio);
            }

            LuuGioHang(gioHang);
            return RedirectToAction("GioHang");
        }

        private List<GioHangItem> LayGioHang()
        {
            var gioHang = HttpContext.Session.GetJson<List<GioHangItem>>("GioHang") ?? new List<GioHangItem>();
            return gioHang;
        }

        private void LuuGioHang(List<GioHangItem> gioHang)
        {
            HttpContext.Session.SetJson("GioHang", gioHang);
        }
    }
} 