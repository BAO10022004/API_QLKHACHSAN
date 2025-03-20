using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class LoaiKhachHang
{
    public int MaLoaiKhach { get; set; }

    public string TenLoaiKhach { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal? PhanTramGiamGia { get; set; }

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
}
