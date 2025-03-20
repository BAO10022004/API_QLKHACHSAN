using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class DatDichVu
{
    public int MaDatDichVu { get; set; }

    public int MaDatPhong { get; set; }

    public int MaDichVu { get; set; }

    public DateTime NgayDatDichVu { get; set; }

    public int? SoLuong { get; set; }

    public decimal? TongTien { get; set; }

    public string? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;

    public virtual DichVu MaDichVuNavigation { get; set; } = null!;
}
