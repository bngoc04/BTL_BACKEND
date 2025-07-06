using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebFood.Models;

namespace WebFood.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<BanAn> BanAns { get; set; }
        public DbSet<DatBan> DatBans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập mối quan hệ giữa các bảng
            modelBuilder.Entity<SanPham>()
                .HasOne(s => s.DanhMuc)
                .WithMany(d => d.SanPhams)
                .HasForeignKey(s => s.DanhMucId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(c => c.DonHang)
                .WithMany(d => d.ChiTietDonHangs)
                .HasForeignKey(c => c.DonHangId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(c => c.SanPham)
                .WithMany(s => s.ChiTietDonHangs)
                .HasForeignKey(c => c.SanPhamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatBan>()
                .HasOne(d => d.BanAn)
                .WithMany()
                .HasForeignKey(d => d.BanAnId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatBan>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatBan>()
                .HasOne(d => d.DonHang)
                .WithOne()
                .HasForeignKey<DatBan>(d => d.DonHangId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
} 