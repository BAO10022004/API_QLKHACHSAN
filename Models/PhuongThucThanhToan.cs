using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class PhuongThucThanhToan
{
    public int MaPhuongThuc { get; set; }

    public string TenPhuongThuc { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
