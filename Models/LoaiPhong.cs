using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class LoaiPhong
{
    public int MaLoaiPhong { get; set; }

    public string TenLoaiPhong { get; set; } = null!;

    public string? MoTa { get; set; }

    public int SoNguoiToiDa { get; set; }

    public int SoTreEmToiDa { get; set; }

    public int TuoiToiDaTreEm { get; set; }

    public decimal GiaCoBan { get; set; }

    public decimal? PhiNguoiThem { get; set; }

    public decimal? PhanTramGiamGiaMacDinh { get; set; }

    public int? GiamGiaLuuTruDai { get; set; }

    public int? GiamGiaDatSom { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<LichSuThayDoiGiaPhong> LichSuThayDoiGiaPhongs { get; set; } = new List<LichSuThayDoiGiaPhong>();

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
