using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class DatPhong
{
    public int MaDatPhong { get; set; }

    public string? MaDatPhongHienThi { get; set; }

    public int MaKhachHang { get; set; }

    public int MaPhong { get; set; }

    public int? MaKenh { get; set; }

    public DateTime NgayNhanPhong { get; set; }

    public DateTime NgayTraPhong { get; set; }

    public int SoNguoiLon { get; set; }

    public int? SoTreEm { get; set; }

    public string? TuoiTreEm { get; set; }

    public DateTime? NgayDat { get; set; }

    public string? TrangThaiDatPhong { get; set; }

    public bool? CoTinhPhiHuy { get; set; }

    public string? LyDoHuy { get; set; }

    public string? YeuCauDacBiet { get; set; }

    public decimal? TongTien { get; set; }

    public int? SoGiuongPhu { get; set; }

    public bool? CoAnSang { get; set; }

    public decimal? PhanTramGiamGia { get; set; }

    public string? LyDoGiamGia { get; set; }

    public int? MaNhanVienDat { get; set; }

    public virtual ICollection<DanhGiaKhachHang> DanhGiaKhachHangs { get; set; } = new List<DanhGiaKhachHang>();

    public virtual ICollection<DatDichVu> DatDichVus { get; set; } = new List<DatDichVu>();

    public virtual KenhDatPhong? MaKenhNavigation { get; set; }

    public virtual KhachHang MaKhachHangNavigation { get; set; } = null!;

    public virtual Phong MaPhongNavigation { get; set; } = null!;

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
