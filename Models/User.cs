using System;
using System.Collections.Generic;

namespace API_QLKHACHSAN.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? LastLogin { get; set; }

    public int? FailedLoginAttempts { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public DateTime? DateCreated { get; set; }

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
