using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFood.Models
{
    public class DonHang
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        public string TenKhachHang { get; set; } = null!;

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        public string DiaChiGiaoHang { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        public string? GhiChu { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TongTien { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;

        public TrangThaiDonHang TrangThai { get; set; } = TrangThaiDonHang.ChoXacNhan;

        // Loại đơn hàng: giao hàng hoặc ăn tại nhà hàng
        public LoaiDonHang LoaiDonHang { get; set; } = LoaiDonHang.GiaoHang;

        public ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
    }

    public enum TrangThaiDonHang
    {
        ChoXacNhan,
        DaXacNhan,
        DangChuanBi,
        DangGiaoHang,
        DaGiaoHang,
        DaHuy
    }

    public enum LoaiDonHang
    {
        GiaoHang,
        AnTaiNhaHang
    }
} 