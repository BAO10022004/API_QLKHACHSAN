using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class LichSuThayDoiGiaPhong
{
    public int MaLichSu { get; set; }

    public int MaLoaiPhong { get; set; }

    public decimal GiaCu { get; set; }

    public decimal GiaMoi { get; set; }

    public DateTime? NgayThayDoi { get; set; }

    public int? NguoiThayDoi { get; set; }

    public string? LyDoThayDoi { get; set; }

    public virtual LoaiPhong MaLoaiPhongNavigation { get; set; } = null!;

    public virtual NhanVien? NguoiThayDoiNavigation { get; set; }
}
