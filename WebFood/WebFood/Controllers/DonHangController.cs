using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFood.Data;
using WebFood.Models;
using WebFood.Models.ViewModels;

namespace WebFood.Controllers
{
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DonHangController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> ThanhToan()
        {
            var gioHang = HttpContext.Session.GetJson<List<GioHangItem>>("GioHang");
            if (gioHang == null || !gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("GioHang", "SanPham");
            }

            // Kiểm tra xem có đang đặt món cho bàn không
            var datBanId = HttpContext.Session.GetInt32("DatBanId");
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

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(DonHang donHang)
        {
            var gioHang = HttpContext.Session.GetJson<List<GioHangItem>>("GioHang");
            if (gioHang == null || !gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("GioHang", "SanPham");
            }

            // Kiểm tra xem có đang đặt món cho bàn không
            var datBanId = HttpContext.Session.GetInt32("DatBanId");
            DatBan? datBan = null;
            
            if (datBanId.HasValue)
            {
                datBan = await _context.DatBans
                    .Include(d => d.BanAn)
                    .FirstOrDefaultAsync(d => d.DatBanId == datBanId.Value);
                
                if (datBan != null)
                {
                    donHang.LoaiDonHang = LoaiDonHang.AnTaiNhaHang;
                    ViewBag.DatBan = datBan;
                }
                else
                {
                    // Nếu không tìm thấy đặt bàn, xóa khỏi session
                    HttpContext.Session.Remove("DatBanId");
                    // Mặc định là giao hàng
                    donHang.LoaiDonHang = LoaiDonHang.GiaoHang;
                }
            }
            else
            {
                // Mặc định là giao hàng
                donHang.LoaiDonHang = LoaiDonHang.GiaoHang;
            }

            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Không thể xác định người dùng hiện tại!";
                return View(donHang);
            }

            donHang.UserId = user.Id;
            
            // Nếu không điền thông tin, lấy từ thông tin người dùng
            if (string.IsNullOrEmpty(donHang.TenKhachHang))
                donHang.TenKhachHang = user.HoTen ?? user.UserName ?? "Khách hàng";
            
            if (string.IsNullOrEmpty(donHang.DiaChiGiaoHang))
                donHang.DiaChiGiaoHang = donHang.LoaiDonHang == LoaiDonHang.AnTaiNhaHang ? 
                    (datBan?.BanAn?.TenBan ?? "Tại nhà hàng") : (user.DiaChi ?? "");
            
            if (string.IsNullOrEmpty(donHang.SoDienThoai))
                donHang.SoDienThoai = user.SoDienThoai ?? "";
            
            if (string.IsNullOrEmpty(donHang.Email))
                donHang.Email = user.Email;

            // Tạo chi tiết đơn hàng từ giỏ hàng
            donHang.ChiTietDonHangs = new List<ChiTietDonHang>();
            decimal tongTien = 0;

            foreach (var item in gioHang)
            {
                var sanPham = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sanPham != null)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        SanPhamId = item.SanPhamId,
                        SoLuong = item.SoLuong,
                        DonGia = sanPham.Gia
                    };
                    donHang.ChiTietDonHangs.Add(chiTiet);
                    tongTien += chiTiet.ThanhTien;
                }
            }

            donHang.TongTien = tongTien;
            donHang.NgayDat = DateTime.Now;
            donHang.TrangThai = TrangThaiDonHang.ChoXacNhan;

            _context.Add(donHang);
            await _context.SaveChangesAsync();

            // Nếu đang đặt món cho bàn, liên kết đơn hàng với đặt bàn
            if (datBanId.HasValue && datBan != null)
            {
                datBan.DonHangId = donHang.Id;
                _context.Update(datBan);
                await _context.SaveChangesAsync();
                
                // Xóa datBanId khỏi session
                HttpContext.Session.Remove("DatBanId");
            }

            // Xóa giỏ hàng sau khi đặt hàng thành công
            HttpContext.Session.Remove("GioHang");

            // Thêm thông báo đặt hàng thành công
            TempData["ThanhToanThanhCong"] = $"Đặt hàng thành công! Đơn hàng #{donHang.Id} với tổng tiền {donHang.TongTien.ToString("N0")} VNĐ đã được xác nhận.";

            return RedirectToAction(nameof(DatHangThanhCong), new { id = donHang.Id });
        }

        [Authorize]
        public async Task<IActionResult> DatHangThanhCong(int id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs!)
                .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null)
            {
                return NotFound();
            }

            // Kiểm tra xem người dùng hiện tại có phải là người đặt hàng không
            var user = await _userManager.GetUserAsync(User);
            if (user == null || (donHang.UserId != user.Id && !User.IsInRole("Admin") && !User.IsInRole("Staff")))
            {
                return Forbid();
            }

            // Thêm thông báo thanh toán thành công
            TempData["ThanhToanThanhCong"] = $"Đặt hàng thành công! Đơn hàng #{donHang.Id} với tổng tiền {donHang.TongTien.ToString("N0")} VNĐ đã được xác nhận.";

            return View(donHang);
        }

        [Authorize]
        public async Task<IActionResult> LichSuDonHang()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var donHangs = await _context.DonHangs
                .Where(d => d.UserId == user.Id)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View(donHangs);
        }

        [Authorize]
        public async Task<IActionResult> ChiTietDonHang(int id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs!)
                .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null)
            {
                return NotFound();
            }

            // Kiểm tra xem người dùng hiện tại có phải là người đặt hàng không
            var user = await _userManager.GetUserAsync(User);
            if (user == null || (donHang.UserId != user.Id && !User.IsInRole("Admin") && !User.IsInRole("Staff")))
            {
                return Forbid();
            }

            // Nếu là đơn hàng ăn tại nhà hàng, kiểm tra xem có liên kết với đặt bàn không
            if (donHang.LoaiDonHang == LoaiDonHang.AnTaiNhaHang)
            {
                var datBan = await _context.DatBans
                    .Include(d => d.BanAn)
                    .FirstOrDefaultAsync(d => d.DonHangId == donHang.Id);
                
                if (datBan != null)
                {
                    ViewBag.DatBan = datBan;
                }
            }

            return View(donHang);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDonHang(int id)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return NotFound();
            }

            // Kiểm tra xem người dùng hiện tại có phải là người đặt hàng không
            var user = await _userManager.GetUserAsync(User);
            if (user == null || (donHang.UserId != user.Id && !User.IsInRole("Admin") && !User.IsInRole("Staff")))
            {
                return Forbid();
            }

            // Chỉ cho phép hủy đơn hàng khi đơn hàng chưa được xác nhận
            if (donHang.TrangThai == TrangThaiDonHang.ChoXacNhan)
            {
                donHang.TrangThai = TrangThaiDonHang.DaHuy;
                _context.Update(donHang);
                
                // Nếu là đơn hàng ăn tại nhà hàng, cập nhật trạng thái đặt bàn
                if (donHang.LoaiDonHang == LoaiDonHang.AnTaiNhaHang)
                {
                    var datBan = await _context.DatBans
                        .Include(d => d.BanAn)
                        .FirstOrDefaultAsync(d => d.DonHangId == donHang.Id);
                    
                    if (datBan != null)
                    {
                        datBan.TrangThai = "Đã hủy";
                        if (datBan.BanAn != null)
                        {
                            datBan.BanAn.DaDat = false;
                        }
                        _context.Update(datBan);
                    }
                }
                
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["Error"] = "Không thể hủy đơn hàng đã được xác nhận!";
            }

            return RedirectToAction(nameof(ChiTietDonHang), new { id = id });
        }
    }
} 