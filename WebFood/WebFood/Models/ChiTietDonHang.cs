using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFood.Models
{
    public class ChiTietDonHang
    {
        public int Id { get; set; }

        public int DonHangId { get; set; }
        public DonHang? DonHang { get; set; }

        public int SanPhamId { get; set; }
        public SanPham? SanPham { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ThanhTien => SoLuong * DonGia;
    }
} 