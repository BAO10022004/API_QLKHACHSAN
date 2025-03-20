using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class ChinhSachHuyPhong
{
    public int MaChinhSach { get; set; }

    public string TenChinhSach { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public int? SoNgayTruocKhiDen { get; set; }

    public decimal? PhanTramPhi { get; set; }

    public bool? TrangThai { get; set; }
}
