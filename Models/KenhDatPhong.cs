using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class KenhDatPhong
{
    public int MaKenh { get; set; }

    public string TenKenh { get; set; } = null!;

    public string LoaiKenh { get; set; } = null!;

    public decimal? PhanTramHoaHong { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
