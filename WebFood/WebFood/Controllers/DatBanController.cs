using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebFood.Data;
using WebFood.Models;

namespace WebFood.Controllers
{
    public class DatBanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatBanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DatBan
        public async Task<IActionResult> Index()
        {
            var banAns = await _context.BanAns.ToListAsync();
            return View(banAns);
        }

        // GET: DatBan/DanhSachBan
        [Authorize]
        public async Task<IActionResult> DanhSachBan()
        {
            var banAns = await _context.BanAns.ToListAsync();
            return View(banAns);
        }

        // GET: DatBan/DatBan/5
        [Authorize]
        public async Task<IActionResult> DatBan(int id)
        {
            var banAn = await _context.BanAns.FindAsync(id);
            if (banAn == null)
            {
                return NotFound();
            }

            if (banAn.DaDat)
            {
                TempData["ErrorMessage"] = "Bàn này đã được đặt.";
                return RedirectToAction(nameof(DanhSachBan));
            }

            var datBan = new DatBan
            {
                BanAnId = banAn.BanAnId,
                BanAn = banAn,
                NgayGioDat = DateTime.Now.AddHours(1),
                SoLuongKhach = 1
            };

            return View(datBan);
        }

        // POST: DatBan/DatBan
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DatBan(DatBan datBan)
        {
            if (ModelState.IsValid)
            {
                var banAn = await _context.BanAns.FindAsync(datBan.BanAnId);
                if (banAn == null)
                {
                    return NotFound();
                }

                if (banAn.DaDat)
                {
                    TempData["ErrorMessage"] = "Bàn này đã được đặt.";
                    return RedirectToAction(nameof(DanhSachBan));
                }

                datBan.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                datBan.TrangThai = "Chờ xác nhận";

                _context.Add(datBan);
                
                // Cập nhật trạng thái bàn
                banAn.DaDat = true;
                _context.Update(banAn);
                
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đặt bàn thành công!";
                return RedirectToAction(nameof(LichSuDatBan));
            }
            return View(datBan);
        }

        // GET: DatBan/LichSuDatBan
        [Authorize]
        public async Task<IActionResult> LichSuDatBan()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var datBans = await _context.DatBans
                .Include(d => d.BanAn)
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.NgayGioDat)
                .ToListAsync();

            return View(datBans);
        }

        // GET: DatBan/ChiTiet/5
        [Authorize]
        public async Task<IActionResult> ChiTiet(int id)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .Include(d => d.DonHang)
                .ThenInclude(dh => dh != null ? dh.ChiTietDonHangs : null)
                .ThenInclude(ct => ct != null ? ct.SanPham : null)
                .FirstOrDefaultAsync(d => d.DatBanId == id);

            if (datBan == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (datBan.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Staff"))
            {
                return Forbid();
            }

            return View(datBan);
        }

        // GET: DatBan/HuyDatBan/5
        [Authorize]
        public async Task<IActionResult> HuyDatBan(int id)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .FirstOrDefaultAsync(d => d.DatBanId == id);

            if (datBan == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (datBan.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Staff"))
            {
                return Forbid();
            }

            if (datBan.TrangThai == "Đã xác nhận" || datBan.TrangThai == "Hoàn thành")
            {
                TempData["ErrorMessage"] = "Không thể hủy đặt bàn đã được xác nhận hoặc hoàn thành.";
                return RedirectToAction(nameof(LichSuDatBan));
            }

            return View(datBan);
        }

        // POST: DatBan/HuyDatBan/5
        [HttpPost, ActionName("HuyDatBan")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> HuyDatBanConfirmed(int id)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .FirstOrDefaultAsync(d => d.DatBanId == id);

            if (datBan == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (datBan.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Staff"))
            {
                return Forbid();
            }

            if (datBan.TrangThai == "Đã xác nhận" || datBan.TrangThai == "Hoàn thành")
            {
                TempData["ErrorMessage"] = "Không thể hủy đặt bàn đã được xác nhận hoặc hoàn thành.";
                return RedirectToAction(nameof(LichSuDatBan));
            }

            datBan.TrangThai = "Đã hủy";
            
            // Cập nhật trạng thái bàn
            if (datBan.BanAn != null)
            {
                datBan.BanAn.DaDat = false;
            }
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Hủy đặt bàn thành công!";
            return RedirectToAction(nameof(LichSuDatBan));
        }

        // GET: DatBan/DatMonTaiBan/5
        [Authorize]
        public async Task<IActionResult> DatMonTaiBan(int id)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .FirstOrDefaultAsync(d => d.DatBanId == id);

            if (datBan == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (datBan.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Staff"))
            {
                return Forbid();
            }

            if (datBan.TrangThai != "Đã xác nhận")
            {
                TempData["ErrorMessage"] = "Chỉ có thể đặt món khi đặt bàn đã được xác nhận.";
                return RedirectToAction(nameof(ChiTiet), new { id = datBan.DatBanId });
            }

            // Chuyển hướng đến trang menu với thông tin đặt bàn
            return RedirectToAction("Index", "SanPham", new { datBanId = datBan.DatBanId });
        }
    }
} 