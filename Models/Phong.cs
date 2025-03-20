using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class Phong
{
    public int MaPhong { get; set; }

    public string SoPhong { get; set; } = null!;

    public int MaLoaiPhong { get; set; }

    public int Tang { get; set; }

    public int? DienTich { get; set; }

    public bool? CoBanCong { get; set; }

    public string? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual LoaiPhong MaLoaiPhongNavigation { get; set; } = null!;
}
