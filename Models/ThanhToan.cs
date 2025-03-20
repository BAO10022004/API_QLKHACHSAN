using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class ThanhToan
{
    public int MaThanhToan { get; set; }

    public int MaDatPhong { get; set; }

    public int MaPhuongThuc { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public decimal SoTien { get; set; }

    public string? MaGiaoDich { get; set; }

    public string? TrangThai { get; set; }

    public string? SoHoaDon { get; set; }

    public bool? XuatHoaDonVat { get; set; }

    public string? TenCongTy { get; set; }

    public string? MaSoThue { get; set; }

    public string? GhiChu { get; set; }

    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;

    public virtual PhuongThucThanhToan MaPhuongThucNavigation { get; set; } = null!;
}
