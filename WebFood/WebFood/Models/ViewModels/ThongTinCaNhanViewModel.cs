using System.ComponentModel.DataAnnotations;

namespace WebFood.Models.ViewModels
{
    public class ThongTinCaNhanViewModel
    {
        public string Id { get; set; } = null!;
        
        [Display(Name = "Tên đăng nhập")]
        public string? UserName { get; set; }
        
        [Display(Name = "Email")]
        public string? Email { get; set; }
        
        [Display(Name = "Họ tên")]
        public string? HoTen { get; set; }
        
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
        
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }
        
        [Display(Name = "Ngày tạo tài khoản")]
        [DataType(DataType.Date)]
        public DateTime NgayTao { get; set; }
        
        [Display(Name = "Vai trò")]
        public string? VaiTro { get; set; }
    }
} 