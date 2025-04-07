using API_QLKHACHSAN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/ThanhToan")]
    [ApiController]
    public class ThanhToanController : Controller
    {
        private readonly QuanLyKhachSanContext dbContext;
        public ThanhToanController(QuanLyKhachSanContext _dbContext)
        {
            this.dbContext = _dbContext ?? new QuanLyKhachSanContext();
        }
        [HttpGet("LayThanhToan")]
        public IActionResult LayThanhToan()
        {
            // Get role
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = dbContext.Users.FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return BadRequest("Must sign in to countinue");
            var roles = user.UserRoles.Select(x => x.Role.RoleName).ToList();
            // Check Vetify
            if (roles.Contains("RECEPTIONIST") || (roles.Contains("MANAGER")) || (roles.Contains("ADMIN")))
            {
                return StatusCode(403, "You do not have permission to get room.");
            }
            List<ThanhToan> listTT = new List<ThanhToan>();
            try
            {
                listTT = dbContext.ThanhToans.ToList();
            }
            catch(Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
            return Ok(new Response() { 
                Messenge = "Get success",
                Data = listTT
            });
        }
        [Authorize]
        [HttpPut("TaoThanhToan")]
        [Authorize]
        public async Task<ActionResult> TaoThanhToan(ThanhToanDTO thanhToanDTO)
        {
            ThanhToan thanhToan = new ThanhToan();
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var user =await dbContext.Users.FirstOrDefaultAsync(u => u.Username == currentUsername);
            var roles = user.UserRoles.Select(x => x.Role.RoleName).ToList();
            if (!roles.Contains("RECEPTIONIST") || !(roles.Contains("MANAGER")) || !(roles.Contains("ADMIN")))
            {
                return StatusCode(403, "You do not have permission to book a room.");
            }
            if (user == null)
            {
                return Unauthorized("Must sign in to continue");
            }
            if (thanhToanDTO == null)
            {
                return BadRequest("Must enter properties order");
            }
            if (dbContext.DatPhongs.FirstOrDefault(p => p.MaDatPhong == thanhToanDTO.MaDatPhong) == null)
            {
                return BadRequest("MaDatPhong not exits");
            }
            if (dbContext.PhuongThucThanhToans.FirstOrDefault(p => p.MaPhuongThuc == thanhToanDTO.MaPhuongThuc) == null)
            {
                return BadRequest("MaPhuongThuc not exits");
            }
            thanhToan.MaDatPhong = thanhToanDTO.MaDatPhong;
            thanhToan.MaPhuongThuc = thanhToanDTO.MaPhuongThuc;
            thanhToan.SoTien = thanhToanDTO.SoTien;
            thanhToan.MaGiaoDich = thanhToanDTO.MaGiaoDich;
            thanhToan.TrangThai = thanhToanDTO.TrangThai;
            thanhToan.TenCongTy = thanhToanDTO.TenCongTy;
            thanhToan.MaSoThue = thanhToanDTO.MaSoThue;
            thanhToan.GhiChu = thanhToanDTO.GhiChu;
            try
            {
                dbContext.ThanhToans.Add(thanhToan);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
            return Ok(new Response()
            {
                Messenge = "ThanhToan success",
                Data = thanhToan
            });
        }
        [HttpPut("XuatHoaDonVAT")]
        public IActionResult XuatHoaDonVAT(int maThanhToan, string maSoThue, string tenCongTy, string ghiChu)
        {
            // Get role
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = dbContext.Users.FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return BadRequest("Must sign in to countinue");
            var roles = user.UserRoles.Select(x => x.Role.RoleName).ToList();
            // Check Vetify
            if (!roles.Contains("RECEPTIONIST") || !(roles.Contains("MANAGER")) || !(roles.Contains("ADMIN")))
            {
                return StatusCode(403, "You do not have permission to get room.");
            }
            var thanhToan = dbContext.ThanhToans.FirstOrDefault(p => p.MaThanhToan == maThanhToan);
            if (thanhToan == null)
            {
                return BadRequest("MaDatPhong not exits");
            }
            if (string.IsNullOrEmpty(maSoThue))
            {
                return BadRequest("MaSoThue is empty");
            }
            if (string.IsNullOrEmpty(tenCongTy))
            {
                return BadRequest("TenCongTy is empty");
            }
            if (string.IsNullOrEmpty(ghiChu))
            {
                return BadRequest("GhiChu is empty");
            }
            thanhToan.MaSoThue = maSoThue;
            thanhToan.TenCongTy = tenCongTy;
            thanhToan.GhiChu = ghiChu;
            thanhToan.XuatHoaDonVat = true;
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
            return Ok(new Response()
            {
                Messenge = "XuatHoaDonVAT success",
                Data = thanhToan
            });
        }
    }
    public class ThanhToanDTO
    {
        public int MaDatPhong { get; set; }
        public int MaPhuongThuc { get; set; }
        public decimal SoTien { get; set; }
        public string? MaGiaoDich { get; set; }
        public string? TrangThai { get; set; }
        public string? TenCongTy { get; set; }
        public string? MaSoThue { get; set; }
        public string? GhiChu { get; set; }

    }

}
