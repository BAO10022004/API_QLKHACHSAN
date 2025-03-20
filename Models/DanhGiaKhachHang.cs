using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class DanhGiaKhachHang
{
    public int MaDanhGia { get; set; }

    public int MaDatPhong { get; set; }

    public int DiemDanhGia { get; set; }

    public string? NhanXet { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public string? NguonDanhGia { get; set; }

    public bool? HienThiCongKhai { get; set; }

    public virtual DatPhong MaDatPhongNavigation { get; set; } = null!;
}
