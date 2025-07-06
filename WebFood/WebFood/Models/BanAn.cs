using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFood.Models
{
    public class BanAn
    {
        [Key]
        public int BanAnId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TenBan { get; set; } = string.Empty;
        
        [Required]
        public int SoLuongChoNgoi { get; set; }
        
        public bool DaDat { get; set; } = false;
        
        public string? MoTa { get; set; }
    }
} 