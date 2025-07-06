using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebFood.Migrations
{
    /// <inheritdoc />
    public partial class AddTableBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoaiDonHang",
                table: "DonHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BanAns",
                columns: table => new
                {
                    BanAnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenBan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoLuongChoNgoi = table.Column<int>(type: "int", nullable: false),
                    DaDat = table.Column<bool>(type: "bit", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanAns", x => x.BanAnId);
                });

            migrationBuilder.CreateTable(
                name: "DatBans",
                columns: table => new
                {
                    DatBanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BanAnId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NgayGioDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongKhach = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonHangId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatBans", x => x.DatBanId);
                    table.ForeignKey(
                        name: "FK_DatBans_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatBans_BanAns_BanAnId",
                        column: x => x.BanAnId,
                        principalTable: "BanAns",
                        principalColumn: "BanAnId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatBans_DonHangs_DonHangId",
                        column: x => x.DonHangId,
                        principalTable: "DonHangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatBans_BanAnId",
                table: "DatBans",
                column: "BanAnId");

            migrationBuilder.CreateIndex(
                name: "IX_DatBans_DonHangId",
                table: "DatBans",
                column: "DonHangId",
                unique: true,
                filter: "[DonHangId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DatBans_UserId",
                table: "DatBans",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatBans");

            migrationBuilder.DropTable(
                name: "BanAns");

            migrationBuilder.DropColumn(
                name: "LoaiDonHang",
                table: "DonHangs");
        }
    }
}
