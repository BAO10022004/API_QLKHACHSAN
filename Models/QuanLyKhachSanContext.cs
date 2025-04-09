using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API_QLKHACHSAN.Models;

public partial class QuanLyKhachSanContext : DbContext
{
    public QuanLyKhachSanContext()
    {
    }
    private readonly IConfiguration _configuration;
    public QuanLyKhachSanContext(DbContextOptions<QuanLyKhachSanContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<ChinhSachHuyPhong> ChinhSachHuyPhongs { get; set; }

    public virtual DbSet<DanhGiaKhachHang> DanhGiaKhachHangs { get; set; }

    public virtual DbSet<DatDichVu> DatDichVus { get; set; }

    public virtual DbSet<DatPhong> DatPhongs { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<KenhDatPhong> KenhDatPhongs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LichSuThayDoiGiaPhong> LichSuThayDoiGiaPhongs { get; set; }

    public virtual DbSet<LoaiKhachHang> LoaiKhachHangs { get; set; }

    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<PhongBan> PhongBans { get; set; }

    public virtual DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChinhSachHuyPhong>(entity =>
        {
            entity.HasKey(e => e.MaChinhSach).HasName("PK__ChinhSac__82663E30228989C6");

            entity.ToTable("ChinhSachHuyPhong");

            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.PhanTramPhi).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenChinhSach).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<DanhGiaKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DanhGiaK__AA9515BFED1FB35B");

            entity.ToTable("DanhGiaKhachHang");

            entity.Property(e => e.HienThiCongKhai).HasDefaultValue(true);
            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguonDanhGia).HasMaxLength(50);
            entity.Property(e => e.NhanXet).HasMaxLength(1000);

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.DanhGiaKhachHangs)
                .HasForeignKey(d => d.MaDatPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DanhGiaKhachHang_DatPhong");
        });

        modelBuilder.Entity<DatDichVu>(entity =>
        {
            entity.HasKey(e => e.MaDatDichVu).HasName("PK__DatDichV__35B4F60A72823DA3");

            entity.ToTable("DatDichVu");

            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.NgayDatDichVu).HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.TongTien).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Đã lên lịch");

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.DatDichVus)
                .HasForeignKey(d => d.MaDatPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatDichVu_DatPhong");

            entity.HasOne(d => d.MaDichVuNavigation).WithMany(p => p.DatDichVus)
                .HasForeignKey(d => d.MaDichVu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatDichVu_DichVu");
        });

        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.MaDatPhong).HasName("PK__DatPhong__6344ADEAF6554A7F");

            entity.ToTable("DatPhong");

            entity.HasIndex(e => e.NgayNhanPhong, "IX_DatPhong_NgayNhanPhong");

            entity.HasIndex(e => e.NgayTraPhong, "IX_DatPhong_NgayTraPhong");

            entity.HasIndex(e => e.TrangThaiDatPhong, "IX_DatPhong_TrangThai");

            entity.Property(e => e.CoAnSang).HasDefaultValue(true);
            entity.Property(e => e.CoTinhPhiHuy).HasDefaultValue(false);
            entity.Property(e => e.LyDoGiamGia).HasMaxLength(200);
            entity.Property(e => e.LyDoHuy).HasMaxLength(200);
            entity.Property(e => e.MaDatPhongHienThi).HasMaxLength(20);
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayNhanPhong).HasColumnType("datetime");
            entity.Property(e => e.NgayTraPhong).HasColumnType("datetime");
            entity.Property(e => e.PhanTramGiamGia)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.SoGiuongPhu).HasDefaultValue(0);
            entity.Property(e => e.SoNguoiLon).HasDefaultValue(1);
            entity.Property(e => e.SoTreEm).HasDefaultValue(0);
            entity.Property(e => e.TongTien).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrangThaiDatPhong)
                .HasMaxLength(20)
                .HasDefaultValue("Đã xác nhận");
            entity.Property(e => e.TuoiTreEm).HasMaxLength(50);
            entity.Property(e => e.YeuCauDacBiet).HasMaxLength(500);

            entity.HasOne(d => d.MaKenhNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaKenh)
                .HasConstraintName("FK_DatPhong_KenhDatPhong");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatPhong_KhachHang");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatPhong_Phong");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaDichVu).HasName("PK__DichVu__C0E6DE8FBADD4C0B");

            entity.ToTable("DichVu");

            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.PhanTramGiamGia)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenDichVu).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<KenhDatPhong>(entity =>
        {
            entity.HasKey(e => e.MaKenh).HasName("PK__KenhDatP__65774D0416375947");

            entity.ToTable("KenhDatPhong");

            entity.Property(e => e.LoaiKenh).HasMaxLength(20);
            entity.Property(e => e.PhanTramHoaHong).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenKenh).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E5E2B9A60A");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.Email, "UQ_KhachHang_Email").IsUnique();

            entity.HasIndex(e => e.SoDienThoai, "UQ_KhachHang_SoDienThoai").IsUnique();

            entity.Property(e => e.DiemTichLuy).HasDefaultValue(0);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.Ho).HasMaxLength(50);
            entity.Property(e => e.LoaiGiayTo).HasMaxLength(20);
            entity.Property(e => e.NgayDangKy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.QuocTich).HasMaxLength(50);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.SoGiayTo).HasMaxLength(30);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MaLoaiKhachNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.MaLoaiKhach)
                .HasConstraintName("FK_KhachHang_LoaiKhachHang");

            entity.HasOne(d => d.User).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_KhachHang_Users");
        });

        modelBuilder.Entity<LichSuThayDoiGiaPhong>(entity =>
        {
            entity.HasKey(e => e.MaLichSu).HasName("PK__LichSuTh__C443222A2A25C8BB");

            entity.ToTable("LichSuThayDoiGiaPhong");

            entity.Property(e => e.GiaCu).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GiaMoi).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LyDoThayDoi).HasMaxLength(200);
            entity.Property(e => e.NgayThayDoi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaLoaiPhongNavigation).WithMany(p => p.LichSuThayDoiGiaPhongs)
                .HasForeignKey(d => d.MaLoaiPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LichSuThayDoiGiaPhong_LoaiPhong");

            entity.HasOne(d => d.NguoiThayDoiNavigation).WithMany(p => p.LichSuThayDoiGiaPhongs)
                .HasForeignKey(d => d.NguoiThayDoi)
                .HasConstraintName("FK_LichSuThayDoiGiaPhong_NhanVien");
        });

        modelBuilder.Entity<LoaiKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaLoaiKhach).HasName("PK__LoaiKhac__87972ADEB902943A");

            entity.ToTable("LoaiKhachHang");

            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.PhanTramGiamGia)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenLoaiKhach).HasMaxLength(50);
        });

        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.HasKey(e => e.MaLoaiPhong).HasName("PK__LoaiPhon__23021217E3281F1F");

            entity.ToTable("LoaiPhong");

            entity.Property(e => e.GiaCoBan).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GiamGiaDatSom).HasDefaultValue(0);
            entity.Property(e => e.GiamGiaLuuTruDai).HasDefaultValue(0);
            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.PhanTramGiamGiaMacDinh)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PhiNguoiThem)
                .HasDefaultValue(600000.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SoNguoiToiDa).HasDefaultValue(2);
            entity.Property(e => e.SoTreEmToiDa).HasDefaultValue(1);
            entity.Property(e => e.TenLoaiPhong).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.TuoiToiDaTreEm).HasDefaultValue(7);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA473413A054");

            entity.ToTable("NhanVien");

            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.DiaChi).HasMaxLength(150);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Ho).HasMaxLength(50);
            entity.Property(e => e.Luong).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NguoiLienHe).HasMaxLength(100);
            entity.Property(e => e.SdtlienHe)
                .HasMaxLength(20)
                .HasColumnName("SDTLienHe");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MaPhongBanNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaPhongBan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhanVien_PhongBan");

            entity.HasOne(d => d.User).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_NhanVien_Users");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__20BD5E5B2C318610");

            entity.ToTable("Phong");

            entity.HasIndex(e => e.SoPhong, "UQ__Phong__7C736CA1956144C4").IsUnique();

            entity.Property(e => e.CoBanCong).HasDefaultValue(false);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.SoPhong).HasMaxLength(10);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Trống");

            entity.HasOne(d => d.MaLoaiPhongNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.MaLoaiPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Phong_LoaiPhong");
        });

        modelBuilder.Entity<PhongBan>(entity =>
        {
            entity.HasKey(e => e.MaPhongBan).HasName("PK__PhongBan__D0910CC8AB17A71D");

            entity.ToTable("PhongBan");

            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenPhongBan).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhuongThucThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaPhuongThuc).HasName("PK__PhuongTh__35F7404EE982A571");

            entity.ToTable("PhuongThucThanhToan");

            entity.Property(e => e.MoTa).HasMaxLength(200);
            entity.Property(e => e.TenPhuongThuc).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A4943EDAF");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160EC158A2A").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B25844A409386A");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.MaGiaoDich).HasMaxLength(100);
            entity.Property(e => e.MaSoThue).HasMaxLength(50);
            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoHoaDon).HasMaxLength(50);
            entity.Property(e => e.SoTien).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TenCongTy).HasMaxLength(100);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Hoàn thành");
            entity.Property(e => e.XuatHoaDonVat)
                .HasDefaultValue(false)
                .HasColumnName("XuatHoaDonVAT");

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDatPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThanhToan_DatPhong");

            entity.HasOne(d => d.MaPhuongThucNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaPhuongThuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThanhToan_PhuongThucThanhToan");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC62AECA00");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4068A1E17").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053462E1A958").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FailedLoginAttempts).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.LockoutEnd).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(128);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A55517019B0");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ_UserRole").IsUnique();

            entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
