using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class DichVu
{
    public int MaDichVu { get; set; }

    public string TenDichVu { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal Gia { get; set; }

    public decimal? PhanTramGiamGia { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DatDichVu> DatDichVus { get; set; } = new List<DatDichVu>();
}
