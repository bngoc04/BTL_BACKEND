using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebFood.Models;

namespace WebFood.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Tạo các vai trò nếu chưa tồn tại
            string[] roleNames = { "Admin", "NhanVien", "KhachHang" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Tạo tài khoản Admin nếu chưa tồn tại
            var adminEmail = "admin@webfood.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    HoTen = "Admin",
                    EmailConfirmed = true,
                    NgayTao = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Tạo tài khoản Nhân viên nếu chưa tồn tại
            var nhanVienEmail = "nhanvien@webfood.com";
            var nhanVienUser = await userManager.FindByEmailAsync(nhanVienEmail);
            if (nhanVienUser == null)
            {
                nhanVienUser = new ApplicationUser
                {
                    UserName = nhanVienEmail,
                    Email = nhanVienEmail,
                    HoTen = "Nhân Viên",
                    DiaChi = "123 Đường ABC, Quận 1, TP.HCM",
                    SoDienThoai = "0901234567",
                    EmailConfirmed = true,
                    NgayTao = DateTime.Now
                };

                var result = await userManager.CreateAsync(nhanVienUser, "NhanVien@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(nhanVienUser, "NhanVien");
                }
            }

            // Tạo tài khoản Khách hàng nếu chưa tồn tại
            var khachHangEmail = "khachhang@example.com";
            var khachHangUser = await userManager.FindByEmailAsync(khachHangEmail);
            if (khachHangUser == null)
            {
                khachHangUser = new ApplicationUser
                {
                    UserName = khachHangEmail,
                    Email = khachHangEmail,
                    HoTen = "Nguyễn Văn A",
                    DiaChi = "456 Đường XYZ, Quận 2, TP.HCM",
                    SoDienThoai = "0909876543",
                    EmailConfirmed = true,
                    NgayTao = DateTime.Now
                };

                var result = await userManager.CreateAsync(khachHangUser, "KhachHang@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(khachHangUser, "KhachHang");
                }
            }

            // Thêm danh mục nếu chưa có dữ liệu
            if (!context.DanhMucs.Any())
            {
                var danhMucs = new List<DanhMuc>
                {
                    new DanhMuc { Ten = "Món chính", MoTa = "Các món ăn chính trong bữa" },
                    new DanhMuc { Ten = "Món phụ", MoTa = "Các món ăn kèm" },
                    new DanhMuc { Ten = "Đồ uống", MoTa = "Các loại nước giải khát" },
                    new DanhMuc { Ten = "Tráng miệng", MoTa = "Các món tráng miệng" },
                    new DanhMuc { Ten = "Đồ ăn nhanh", MoTa = "Các món ăn nhanh" }
                };

                await context.DanhMucs.AddRangeAsync(danhMucs);
                await context.SaveChangesAsync();
            }

            // Thêm bàn ăn nếu chưa có dữ liệu
            if (!context.BanAns.Any())
            {
                var banAns = new List<BanAn>
                {
                    new BanAn { TenBan = "Bàn 1", SoLuongChoNgoi = 2, DaDat = false, MoTa = "Bàn đôi gần cửa sổ" },
                    new BanAn { TenBan = "Bàn 2", SoLuongChoNgoi = 2, DaDat = false, MoTa = "Bàn đôi gần cửa sổ" },
                    new BanAn { TenBan = "Bàn 3", SoLuongChoNgoi = 4, DaDat = false, MoTa = "Bàn gia đình" },
                    new BanAn { TenBan = "Bàn 4", SoLuongChoNgoi = 4, DaDat = false, MoTa = "Bàn gia đình" },
                    new BanAn { TenBan = "Bàn 5", SoLuongChoNgoi = 6, DaDat = false, MoTa = "Bàn lớn cho nhóm" },
                    new BanAn { TenBan = "Bàn 6", SoLuongChoNgoi = 6, DaDat = false, MoTa = "Bàn lớn cho nhóm" },
                    new BanAn { TenBan = "Bàn 7", SoLuongChoNgoi = 8, DaDat = false, MoTa = "Bàn lớn cho nhóm đông" },
                    new BanAn { TenBan = "Bàn 8", SoLuongChoNgoi = 10, DaDat = false, MoTa = "Bàn VIP cho nhóm lớn" },
                    new BanAn { TenBan = "Bàn 9", SoLuongChoNgoi = 2, DaDat = false, MoTa = "Bàn đôi khu vực yên tĩnh" },
                    new BanAn { TenBan = "Bàn 10", SoLuongChoNgoi = 4, DaDat = false, MoTa = "Bàn gia đình khu vực yên tĩnh" }
                };

                await context.BanAns.AddRangeAsync(banAns);
                await context.SaveChangesAsync();
            }

            // Thêm sản phẩm nếu chưa có dữ liệu
            if (!context.SanPhams.Any())
            {
                var danhMucs = await context.DanhMucs.ToListAsync();
                
                var sanPhams = new List<SanPham>
                {
                    // Món chính
                    new SanPham
                    {
                        Ten = "Cơm gà xối mỡ",
                        MoTa = "Cơm gà xối mỡ thơm ngon, gà giòn bên ngoài, mềm bên trong",
                        Gia = 45000,
                        HinhAnh = "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Món chính").Id
                    },
                    new SanPham
                    {
                        Ten = "Bún bò Huế",
                        MoTa = "Bún bò Huế cay nồng, thơm ngon đặc trưng của xứ Huế",
                        Gia = 50000,
                        HinhAnh = "https://images.unsplash.com/photo-1576577445504-6af96477db52?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Món chính").Id
                    },
                    new SanPham
                    {
                        Ten = "Phở bò tái",
                        MoTa = "Phở bò tái với nước dùng đậm đà, thịt bò mềm",
                        Gia = 45000,
                        HinhAnh = "https://images.unsplash.com/photo-1503764654157-72d979d9af2f?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Món chính").Id
                    },
                    new SanPham
                    {
                        Ten = "Cơm tấm sườn bì chả",
                        MoTa = "Cơm tấm với sườn nướng, bì và chả trứng truyền thống",
                        Gia = 55000,
                        HinhAnh = "https://images.unsplash.com/photo-1569058242253-92a9c755a0ec?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Món chính").Id
                    },

                    // Món phụ
                    new SanPham
                    {
                        Ten = "Gỏi cuốn tôm thịt",
                        MoTa = "Gỏi cuốn tươi mát với tôm, thịt và rau sống",
                        Gia = 35000,
                        HinhAnh = "https://images.unsplash.com/photo-1553530979-fbb9e4aee36f?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Món phụ").Id
                    },
                    new SanPham
                    {
                        Ten = "Chả giò",
                        MoTa = "Chả giò giòn rụm với nhân thịt và nấm",
                        Gia = 30000,
                        HinhAnh = "https://images.unsplash.com/photo-1625938145744-e380810e0a2e?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Món phụ").Id
                    },
                    new SanPham
                    {
                        Ten = "Salad trộn",
                        MoTa = "Salad trộn với nhiều loại rau củ tươi ngon",
                        Gia = 25000,
                        HinhAnh = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Món phụ").Id
                    },

                    // Đồ uống
                    new SanPham
                    {
                        Ten = "Trà đào cam sả",
                        MoTa = "Trà đào thơm mát với cam và sả",
                        Gia = 25000,
                        HinhAnh = "https://images.unsplash.com/photo-1556679343-c1306ee5277b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Đồ uống").Id
                    },
                    new SanPham
                    {
                        Ten = "Cà phê sữa đá",
                        MoTa = "Cà phê sữa đá đậm đà, thơm ngon",
                        Gia = 20000,
                        HinhAnh = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Đồ uống").Id
                    },
                    new SanPham
                    {
                        Ten = "Sinh tố bơ",
                        MoTa = "Sinh tố bơ béo ngậy, thơm ngon",
                        Gia = 30000,
                        HinhAnh = "https://images.unsplash.com/photo-1623065422902-30a2d299bbe4?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Đồ uống").Id
                    },

                    // Tráng miệng
                    new SanPham
                    {
                        Ten = "Chè thái",
                        MoTa = "Chè thái thơm ngon với nhiều loại thạch",
                        Gia = 25000,
                        HinhAnh = "https://images.unsplash.com/photo-1563805042-7684c019e1cb?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Tráng miệng").Id
                    },
                    new SanPham
                    {
                        Ten = "Bánh flan",
                        MoTa = "Bánh flan mềm mịn, thơm ngon",
                        Gia = 15000,
                        HinhAnh = "https://images.unsplash.com/photo-1586788224331-947f68671cf1?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Tráng miệng").Id
                    },
                    new SanPham
                    {
                        Ten = "Chè đậu xanh",
                        MoTa = "Chè đậu xanh thơm ngon, bổ dưỡng",
                        Gia = 20000,
                        HinhAnh = "https://images.unsplash.com/photo-1553423098-c73228e700c2?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Tráng miệng").Id
                    },

                    // Đồ ăn nhanh
                    new SanPham
                    {
                        Ten = "Bánh mì thịt",
                        MoTa = "Bánh mì thịt thơm ngon với nhiều loại rau và sốt",
                        Gia = 25000,
                        HinhAnh = "https://images.unsplash.com/photo-1600628421055-4d30de868b8f?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Đồ ăn nhanh").Id
                    },
                    new SanPham
                    {
                        Ten = "Xôi gà",
                        MoTa = "Xôi gà thơm ngon, đầy đặn",
                        Gia = 30000,
                        HinhAnh = "https://images.unsplash.com/photo-1603133872878-684f208fb84b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Đồ ăn nhanh").Id
                    },
                    new SanPham
                    {
                        Ten = "Hamburger bò",
                        MoTa = "Hamburger bò thơm ngon với nhiều loại rau và sốt",
                        Gia = 35000,
                        HinhAnh = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=500&h=300&q=80",
                        ConHang = true,
                        DanhMucId = danhMucs.First(d => d.Ten == "Đồ ăn nhanh").Id
                    }
                };

                await context.SanPhams.AddRangeAsync(sanPhams);
                await context.SaveChangesAsync();
            }
        }
    }
} 