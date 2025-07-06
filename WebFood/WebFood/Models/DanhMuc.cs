using System.ComponentModel.DataAnnotations;

namespace WebFood.Models
{
    public class DanhMuc
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Ten { get; set; } = null!;

        public string? MoTa { get; set; }

        public ICollection<SanPham>? SanPhams { get; set; }
    }
} 