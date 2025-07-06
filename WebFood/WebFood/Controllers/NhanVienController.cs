using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFood.Data;
using WebFood.Models;
using WebFood.Models.ViewModels;

namespace WebFood.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class NhanVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NhanVienController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DanhSachBan()
        {
            var banAns = await _context.BanAns.ToListAsync();
            return View(banAns);
        }

        public async Task<IActionResult> DanhSachDatBan()
        {
            var datBans = await _context.DatBans
                .Include(d => d.BanAn)
                .Include(d => d.User)
                .OrderByDescending(d => d.NgayGioDat)
                .ToListAsync();
            
            return View(datBans);
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanDatBan(int id)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .FirstOrDefaultAsync(d => d.DatBanId == id);
            
            if (datBan == null)
            {
                return NotFound();
            }
            
            datBan.TrangThai = "Đã xác nhận";
            _context.Update(datBan);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Đã xác nhận đặt bàn thành công!";
            return RedirectToAction(nameof(DanhSachDatBan));
        }

        [HttpPost]
        public async Task<IActionResult> HuyDatBan(int id)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .FirstOrDefaultAsync(d => d.DatBanId == id);
            
            if (datBan == null)
            {
                return NotFound();
            }
            
            datBan.TrangThai = "Đã hủy";
            
            // Cập nhật trạng thái bàn
            if (datBan.BanAn != null)
            {
                datBan.BanAn.DaDat = false;
            }
            
            _context.Update(datBan);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Đã hủy đặt bàn thành công!";
            return RedirectToAction(nameof(DanhSachDatBan));
        }

        [HttpPost]
        public async Task<IActionResult> HoanThanhDatBan(int id)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .FirstOrDefaultAsync(d => d.DatBanId == id);
            
            if (datBan == null)
            {
                return NotFound();
            }
            
            datBan.TrangThai = "Hoàn thành";
            
            // Cập nhật trạng thái bàn
            if (datBan.BanAn != null)
            {
                datBan.BanAn.DaDat = false;
            }
            
            _context.Update(datBan);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Đã hoàn thành đặt bàn!";
            return RedirectToAction(nameof(DanhSachDatBan));
        }

        public async Task<IActionResult> ChiTietDatBan(int id)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .Include(d => d.User)
                .Include(d => d.DonHang)
                .ThenInclude(dh => dh != null ? dh.ChiTietDonHangs : null)
                .ThenInclude(ct => ct != null ? ct.SanPham : null)
                .FirstOrDefaultAsync(d => d.DatBanId == id);
            
            if (datBan == null)
            {
                return NotFound();
            }
            
            return View(datBan);
        }

        public async Task<IActionResult> DatMon(int soBan, int? datBanId = null)
        {
            ViewBag.SoBan = soBan;
            
            if (datBanId.HasValue)
            {
                var datBan = await _context.DatBans
                    .Include(d => d.BanAn)
                    .FirstOrDefaultAsync(d => d.DatBanId == datBanId.Value);
                
                if (datBan != null)
                {
                    ViewBag.DatBan = datBan;
                }
            }
            
            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
            var sanPhams = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .ToListAsync();
            
            return View(sanPhams);
        }

        [HttpPost]
        public async Task<IActionResult> ThemVaoHoaDon(int soBan, int sanPhamId, int soLuong, int? datBanId = null, bool stayOnPage = true)
        {
            var sanPham = await _context.SanPhams.FindAsync(sanPhamId);
            if (sanPham == null)
            {
                return NotFound();
            }

            // Lấy hoặc tạo mới hóa đơn cho bàn
            var hoaDon = HttpContext.Session.GetJson<HoaDonViewModel>($"HoaDon_Ban_{soBan}") ?? new HoaDonViewModel
            {
                SoBan = soBan,
                NgayTao = DateTime.Now,
                ChiTietHoaDon = new List<ChiTietHoaDonViewModel>()
            };

            // Nếu có datBanId, lưu vào hóa đơn
            if (datBanId.HasValue)
            {
                hoaDon.DatBanId = datBanId.Value;
            }

            // Kiểm tra xem sản phẩm đã có trong hóa đơn chưa
            var chiTiet = hoaDon.ChiTietHoaDon.FirstOrDefault(c => c.SanPhamId == sanPhamId);
            if (chiTiet != null)
            {
                chiTiet.SoLuong += soLuong;
            }
            else
            {
                hoaDon.ChiTietHoaDon.Add(new ChiTietHoaDonViewModel
                {
                    SanPhamId = sanPham.Id,
                    TenSanPham = sanPham.Ten,
                    DonGia = sanPham.Gia,
                    SoLuong = soLuong
                });
            }

            // Tính lại tổng tiền
            hoaDon.TongTien = hoaDon.ChiTietHoaDon.Sum(c => c.ThanhTien);

            // Lưu lại vào session
            HttpContext.Session.SetJson($"HoaDon_Ban_{soBan}", hoaDon);

            // Hiển thị thông báo thành công
            TempData["Success"] = $"Đã thêm {soLuong} {sanPham.Ten} vào giỏ hàng!";
            
            // Nếu stayOnPage = true, ở lại trang hiện tại, ngược lại chuyển đến trang hóa đơn
            if (stayOnPage)
            {
                return RedirectToAction(nameof(DatMon), new { soBan = soBan, datBanId = datBanId });
            }
            else
            {
                return RedirectToAction(nameof(XemHoaDon), new { soBan = soBan });
            }
        }

        public async Task<IActionResult> XemHoaDon(int soBan)
        {
            var hoaDon = HttpContext.Session.GetJson<HoaDonViewModel>($"HoaDon_Ban_{soBan}");
            if (hoaDon == null)
            {
                hoaDon = new HoaDonViewModel
                {
                    SoBan = soBan,
                    NgayTao = DateTime.Now,
                    ChiTietHoaDon = new List<ChiTietHoaDonViewModel>()
                };
                HttpContext.Session.SetJson($"HoaDon_Ban_{soBan}", hoaDon);
            }

            // Nếu có DatBanId, lấy thông tin đặt bàn
            if (hoaDon.DatBanId.HasValue)
            {
                var datBan = await _context.DatBans
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.DatBanId == hoaDon.DatBanId.Value);
                
                if (datBan != null)
                {
                    ViewBag.DatBan = datBan;
                }
            }

            ViewBag.SoBan = soBan;
            return View(hoaDon);
        }

        [HttpPost]
        public IActionResult CapNhatSoLuong(int soBan, int sanPhamId, int soLuong)
        {
            var hoaDon = HttpContext.Session.GetJson<HoaDonViewModel>($"HoaDon_Ban_{soBan}");
            if (hoaDon == null)
            {
                return NotFound();
            }

            var chiTiet = hoaDon.ChiTietHoaDon.FirstOrDefault(c => c.SanPhamId == sanPhamId);
            if (chiTiet == null)
            {
                return NotFound();
            }

            if (soLuong > 0)
            {
                chiTiet.SoLuong = soLuong;
            }
            else
            {
                hoaDon.ChiTietHoaDon.Remove(chiTiet);
            }

            // Tính lại tổng tiền
            hoaDon.TongTien = hoaDon.ChiTietHoaDon.Sum(c => c.ThanhTien);

            // Lưu lại vào session
            HttpContext.Session.SetJson($"HoaDon_Ban_{soBan}", hoaDon);

            return RedirectToAction(nameof(XemHoaDon), new { soBan = soBan });
        }

        [HttpPost]
        public IActionResult XoaMonKhoiHoaDon(int soBan, int sanPhamId)
        {
            var hoaDon = HttpContext.Session.GetJson<HoaDonViewModel>($"HoaDon_Ban_{soBan}");
            if (hoaDon == null)
            {
                return NotFound();
            }

            var chiTiet = hoaDon.ChiTietHoaDon.FirstOrDefault(c => c.SanPhamId == sanPhamId);
            if (chiTiet != null)
            {
                hoaDon.ChiTietHoaDon.Remove(chiTiet);
            }

            // Tính lại tổng tiền
            hoaDon.TongTien = hoaDon.ChiTietHoaDon.Sum(c => c.ThanhTien);

            // Lưu lại vào session
            HttpContext.Session.SetJson($"HoaDon_Ban_{soBan}", hoaDon);

            return RedirectToAction(nameof(XemHoaDon), new { soBan = soBan });
        }

        [HttpPost]
        public async Task<IActionResult> ThanhToan(int soBan)
        {
            var hoaDon = HttpContext.Session.GetJson<HoaDonViewModel>($"HoaDon_Ban_{soBan}");
            if (hoaDon == null || !hoaDon.ChiTietHoaDon.Any())
            {
                TempData["Error"] = "Không có món ăn nào trong hóa đơn!";
                return RedirectToAction(nameof(XemHoaDon), new { soBan = soBan });
            }

            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Không thể xác định người dùng hiện tại!";
                return RedirectToAction(nameof(XemHoaDon), new { soBan = soBan });
            }

            // Tạo đơn hàng mới
            var donHang = new DonHang
            {
                TenKhachHang = $"Khách hàng tại bàn {soBan}",
                DiaChiGiaoHang = $"Bàn số {soBan}",
                SoDienThoai = "Không có",
                Email = "khachhangtaiban@example.com",
                GhiChu = $"Đơn hàng tại bàn {soBan}",
                TongTien = hoaDon.TongTien,
                NgayDat = DateTime.Now,
                TrangThai = TrangThaiDonHang.DaXacNhan,
                UserId = user.Id,
                LoaiDonHang = LoaiDonHang.AnTaiNhaHang,
                ChiTietDonHangs = hoaDon.ChiTietHoaDon.Select(c => new ChiTietDonHang
                {
                    SanPhamId = c.SanPhamId,
                    SoLuong = c.SoLuong,
                    DonGia = c.DonGia
                }).ToList()
            };

            _context.Add(donHang);
            await _context.SaveChangesAsync();

            // Nếu có DatBanId, liên kết đơn hàng với đặt bàn
            if (hoaDon.DatBanId.HasValue)
            {
                var datBan = await _context.DatBans.FindAsync(hoaDon.DatBanId.Value);
                if (datBan != null)
                {
                    datBan.DonHangId = donHang.Id;
                    datBan.TrangThai = "Hoàn thành";
                    _context.Update(datBan);
                    await _context.SaveChangesAsync();
                }
            }

            // Xóa hóa đơn khỏi session
            HttpContext.Session.Remove($"HoaDon_Ban_{soBan}");

            TempData["ThanhToanThanhCong"] = $"Thanh toán thành công hóa đơn bàn {soBan} với tổng tiền {hoaDon.TongTien.ToString("N0")} VNĐ!";
            return RedirectToAction(nameof(DanhSachBan));
        }

        public async Task<IActionResult> DanhSachHoaDon()
        {
            var donHangs = await _context.DonHangs
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
            
            return View(donHangs);
        }

        public async Task<IActionResult> ChiTietHoaDon(int id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs!)
                .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
    }

    public class BanViewModel
    {
        public int SoBan { get; set; }
        public TrangThaiBan TrangThai { get; set; }
    }

    public enum TrangThaiBan
    {
        Trong,
        DaDat,
        DangPhucVu
    }

    public class HoaDonViewModel
    {
        public int SoBan { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public int? DatBanId { get; set; }
        public List<ChiTietHoaDonViewModel> ChiTietHoaDon { get; set; } = new List<ChiTietHoaDonViewModel>();
    }

    public class ChiTietHoaDonViewModel
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = null!;
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
} 