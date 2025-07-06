using Microsoft.AspNetCore.Identity;

namespace WebFood.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? HoTen { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
} 