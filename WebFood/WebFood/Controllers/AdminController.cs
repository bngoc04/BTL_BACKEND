using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using WebFood.Data;
using WebFood.Models;

namespace WebFood.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Quản lý người dùng
        public async Task<IActionResult> QuanLyNguoiDung()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    HoTen = user.HoTen,
                    DiaChi = user.DiaChi,
                    SoDienThoai = user.SoDienThoai,
                    NgayTao = user.NgayTao,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> ChiTietNguoiDung(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                HoTen = user.HoTen,
                DiaChi = user.DiaChi,
                SoDienThoai = user.SoDienThoai,
                NgayTao = user.NgayTao,
                Roles = roles.ToList()
            };

            return View(userViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ChinhSuaVaiTro(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleViewModel = new RoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(r => new RoleItem
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    IsSelected = userRoles.Contains(r.Name)
                }).ToList()
            };

            return View(roleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChinhSuaVaiTro(RoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();
            await _userManager.AddToRolesAsync(user, selectedRoles);

            return RedirectToAction(nameof(QuanLyNguoiDung));
        }
        #endregion

        #region Quản lý danh mục
        public async Task<IActionResult> QuanLyDanhMuc()
        {
            return View(await _context.DanhMucs.ToListAsync());
        }

        [HttpGet]
        public IActionResult ThemDanhMuc()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemDanhMuc([Bind("Ten,MoTa")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhMuc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(QuanLyDanhMuc));
            }
            return View(danhMuc);
        }

        [HttpGet]
        public async Task<IActionResult> SuaDanhMuc(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaDanhMuc(int id, [Bind("Id,Ten,MoTa")] DanhMuc danhMuc)
        {
            if (id != danhMuc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhMuc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhMucExists(danhMuc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(QuanLyDanhMuc));
            }
            return View(danhMuc);
        }

        [HttpGet]
        public async Task<IActionResult> XoaDanhMuc(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc);
        }

        [HttpPost, ActionName("XoaDanhMuc")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanXoaDanhMuc(int id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            _context.DanhMucs.Remove(danhMuc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(QuanLyDanhMuc));
        }

        private bool DanhMucExists(int id)
        {
            return _context.DanhMucs.Any(e => e.Id == id);
        }
        #endregion

        #region Quản lý sản phẩm
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> QuanLySanPham()
        {
            var sanPhams = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .ToListAsync();
            return View(sanPhams);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpGet]
        public IActionResult ThemSanPham()
        {
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View();
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSanPham([Bind("Ten,MoTa,Gia,HinhAnh,ConHang,DanhMucId")] SanPham sanPham, IFormFile hinhAnh)
        {
            if (ModelState.IsValid)
            {
                if (hinhAnh != null && hinhAnh.Length > 0)
                {
                    // Xử lý upload hình ảnh
                    sanPham.HinhAnh = await UploadHinhAnh(hinhAnh);
                }

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(QuanLySanPham));
            }
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(sanPham);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpGet]
        public async Task<IActionResult> SuaSanPham(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(sanPham);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaSanPham(int id, [Bind("Id,Ten,MoTa,Gia,HinhAnh,ConHang,DanhMucId")] SanPham sanPham, IFormFile? hinhAnh)
        {
            if (id != sanPham.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (hinhAnh != null && hinhAnh.Length > 0)
                    {
                        // Xử lý upload hình ảnh
                        sanPham.HinhAnh = await UploadHinhAnh(hinhAnh);
                    }

                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(QuanLySanPham));
            }
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(sanPham);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpGet]
        public async Task<IActionResult> XoaSanPham(int? id)
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

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpPost, ActionName("XoaSanPham")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanXoaSanPham(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(QuanLySanPham));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.Id == id);
        }
        #endregion

        #region Quản lý đơn hàng
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> QuanLyDonHang()
        {
            var donHangs = await _context.DonHangs
                .Include(d => d.User)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
            return View(donHangs);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> ChiTietDonHang(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.User)
                .Include(d => d.ChiTietDonHangs!)
                .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThaiDonHang(int id, TrangThaiDonHang trangThai)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return NotFound();
            }

            donHang.TrangThai = trangThai;
            _context.Update(donHang);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ChiTietDonHang), new { id = id });
        }
        #endregion

        #region Quản lý bàn ăn
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> QuanLyBanAn()
        {
            var banAns = await _context.BanAns.ToListAsync();
            return View(banAns);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpGet]
        public IActionResult ThemBanAn()
        {
            return View();
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemBanAn([Bind("TenBan,SoLuongChoNgoi,MoTa")] BanAn banAn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(banAn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(QuanLyBanAn));
            }
            return View(banAn);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpGet]
        public async Task<IActionResult> SuaBanAn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banAn = await _context.BanAns.FindAsync(id);
            if (banAn == null)
            {
                return NotFound();
            }
            return View(banAn);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaBanAn(int id, [Bind("BanAnId,TenBan,SoLuongChoNgoi,DaDat,MoTa")] BanAn banAn)
        {
            if (id != banAn.BanAnId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(banAn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BanAnExists(banAn.BanAnId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(QuanLyBanAn));
            }
            return View(banAn);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpGet]
        public async Task<IActionResult> XoaBanAn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banAn = await _context.BanAns
                .FirstOrDefaultAsync(m => m.BanAnId == id);
            if (banAn == null)
            {
                return NotFound();
            }

            return View(banAn);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpPost, ActionName("XoaBanAn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanXoaBanAn(int id)
        {
            var banAn = await _context.BanAns.FindAsync(id);
            if (banAn == null)
            {
                return NotFound();
            }
            
            // Kiểm tra xem bàn có đang được đặt không
            var datBan = await _context.DatBans.FirstOrDefaultAsync(d => d.BanAnId == id && d.TrangThai != "Đã hủy" && d.TrangThai != "Hoàn thành");
            if (datBan != null)
            {
                TempData["Error"] = "Không thể xóa bàn đang được đặt!";
                return RedirectToAction(nameof(QuanLyBanAn));
            }
            
            _context.BanAns.Remove(banAn);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(QuanLyBanAn));
        }

        private bool BanAnExists(int id)
        {
            return _context.BanAns.Any(e => e.BanAnId == id);
        }
        #endregion

        #region Quản lý đặt bàn
        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> QuanLyDatBan()
        {
            var datBans = await _context.DatBans
                .Include(d => d.BanAn)
                .Include(d => d.User)
                .Include(d => d.DonHang)
                .OrderByDescending(d => d.NgayGioDat)
                .ToListAsync();
            
            return View(datBans);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> ChiTietDatBan(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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

        [Authorize(Roles = "Admin,NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThaiDatBan(int id, string trangThai)
        {
            var datBan = await _context.DatBans
                .Include(d => d.BanAn)
                .FirstOrDefaultAsync(d => d.DatBanId == id);
            
            if (datBan == null)
            {
                return NotFound();
            }
            
            datBan.TrangThai = trangThai;
            
            // Cập nhật trạng thái bàn
            if (datBan.BanAn != null)
            {
                if (trangThai == "Đã hủy" || trangThai == "Hoàn thành")
                {
                    datBan.BanAn.DaDat = false;
                }
                else
                {
                    datBan.BanAn.DaDat = true;
                }
            }
            
            _context.Update(datBan);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(QuanLyDatBan));
        }
        #endregion

        private async Task<string> UploadHinhAnh(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            // Tạo tên file duy nhất để tránh trùng lặp
            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            
            // Đường dẫn lưu file
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            
            // Đảm bảo thư mục tồn tại
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            
            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // Trả về đường dẫn tương đối để lưu vào database
            return $"/images/{fileName}";
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? HoTen { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        public DateTime NgayTao { get; set; }
        public List<string>? Roles { get; set; }
    }

    public class RoleViewModel
    {
        public string UserId { get; set; } = null!;
        public string? UserName { get; set; }
        public List<RoleItem>? Roles { get; set; }
    }

    public class RoleItem
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
} 