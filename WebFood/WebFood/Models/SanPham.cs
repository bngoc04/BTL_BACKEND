using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFood.Models
{
    public class SanPham
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string Ten { get; set; } = null!;

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Gia { get; set; }

        public string? HinhAnh { get; set; }

        public bool ConHang { get; set; } = true;

        public int DanhMucId { get; set; }
        public DanhMuc? DanhMuc { get; set; }

        public ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
    }
} 