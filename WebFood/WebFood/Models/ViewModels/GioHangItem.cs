using System.ComponentModel.DataAnnotations.Schema;

namespace WebFood.Models.ViewModels
{
    public class GioHangItem
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = null!;
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string? HinhAnh { get; set; }

        [NotMapped]
        public decimal ThanhTien => SoLuong * DonGia;
    }
} 