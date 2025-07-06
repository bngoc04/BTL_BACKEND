using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFood.Models
{
    public class DatBan
    {
        [Key]
        public int DatBanId { get; set; }
        
        public int BanAnId { get; set; }
        [ForeignKey("BanAnId")]
        public BanAn? BanAn { get; set; }
        
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        
        [Required]
        public DateTime NgayGioDat { get; set; }
        
        [Required]
        public int SoLuongKhach { get; set; }
        
        public string? GhiChu { get; set; }
        
        [Required]
        public string TrangThai { get; set; } = "Chờ xác nhận"; // Chờ xác nhận, Đã xác nhận, Đã hủy, Hoàn thành
        
        // Liên kết với đơn hàng nếu có
        public int? DonHangId { get; set; }
        [ForeignKey("DonHangId")]
        public DonHang? DonHang { get; set; }
    }
} 