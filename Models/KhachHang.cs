using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class KhachHang
{
    public int MaKhachHang { get; set; }

    public string Ho { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? QuocTich { get; set; }

    public string LoaiGiayTo { get; set; } = null!;

    public string SoGiayTo { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public int? MaLoaiKhach { get; set; }

    public DateTime? NgayDangKy { get; set; }

    public int? DiemTichLuy { get; set; }

    public string? GhiChu { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual LoaiKhachHang? MaLoaiKhachNavigation { get; set; }

    public virtual User? User { get; set; }
}
