using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class NhanVien
{
    public int MaNhanVien { get; set; }

    public string Ho { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public int MaPhongBan { get; set; }

    public string ChucVu { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public DateOnly NgayVaoLam { get; set; }

    public decimal? Luong { get; set; }

    public string? DiaChi { get; set; }

    public string? NguoiLienHe { get; set; }

    public string? SdtlienHe { get; set; }

    public bool? TrangThai { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<LichSuThayDoiGiaPhong> LichSuThayDoiGiaPhongs { get; set; } = new List<LichSuThayDoiGiaPhong>();

    public virtual PhongBan MaPhongBanNavigation { get; set; } = null!;

    public virtual User? User { get; set; }
}
