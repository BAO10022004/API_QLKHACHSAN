﻿using API_QLKHACHSAN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/KhachHang")]
    [ApiController]
    public class KhachHangController : Controller
    {
        readonly private QuanLyKhachSanContext dbContext;
        public KhachHangController(QuanLyKhachSanContext _dbContext)
        {
            this.dbContext = _dbContext ?? new QuanLyKhachSanContext();
        }
        [HttpPut("TaoTaiKhoang")]
        public IActionResult TaoTaiKhoang([FromBody] TaiKhoangDTO req)
        {
            if (req == null)
                return BadRequest("TaoTaiKhoang fail");
            if (string.IsNullOrEmpty(req.TenDayDu))
                return BadRequest("TenDayDu is empty");
            if (string.IsNullOrEmpty(req.Email) && string.IsNullOrEmpty(req.SoDienThoai))
                return BadRequest("Must input Email or SoDienThoai");
            if (string.IsNullOrEmpty(req.LoaiGiayTo) || string.IsNullOrEmpty(req.SoGiayTo))
                return BadRequest("Must input LoaiGiayTo or SoGiayTo");

            KhachHang kh = new KhachHang();
            kh.Ten = req.TenDayDu.Split(" ").Last();
            int lastSpaceIndex = req.TenDayDu.LastIndexOf(' '); 
            kh.Ho = lastSpaceIndex > 0 ? req.TenDayDu.Substring(0, lastSpaceIndex) : "";

            if (!Regex.IsMatch(req.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return BadRequest("Email incorrect");
            }
            kh.Email = req.Email;
            if(dbContext.KhachHangs.FirstOrDefault(x=> x.SoDienThoai.Equals(req.SoDienThoai))  != null)
            {
                return BadRequest("SoDienThoai exits");
            }
            kh.SoDienThoai = req.SoDienThoai;
            kh.QuocTich = req.QuocTich;
            kh.LoaiGiayTo = req.LoaiGiayTo;

            if (dbContext.KhachHangs.FirstOrDefault(x => x.SoGiayTo.Equals(req.SoGiayTo)) != null)
            {
                return BadRequest("SoGiayTo exits");
            }

            kh.SoGiayTo = req.SoGiayTo;
            try
            {
                kh.NgaySinh  = DateOnly.ParseExact(req.NgaySinh, "dd-MM-yyyy");
            }
            catch(Exception)
            {
                return BadRequest("NgaySinh incorrect");
            }
            
            kh.MaLoaiKhach = 2;// normal
            kh.NgayDangKy = DateTime.Now;
            try
            {
                dbContext.KhachHangs.Add(kh);
                dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                return BadRequest($"Create fail : {ex.Message}");
            }
            return Ok(new  Response (){Messege = "Create account success",Data =  kh });
        }
        [HttpGet("GetKhachHang")]
        [Authorize]
        public IActionResult GetListKhachHang()
        {
            // Get role
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = dbContext.Users.FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return BadRequest("Must sign in to countinue");
            var roles = dbContext.UserRoles.Where(u => u.UserId == user.UserId).ToList().Select(u => u.RoleId);
            // Check Vetify
             if (!roles.Contains(2) && !(roles.Contains(3)) && !(roles.Contains(7)))
            {
                return StatusCode(403, "You do not have permission to get room.");
            }
            var listRoom = dbContext.KhachHangs.ToList();
            if (listRoom == null)
                return BadRequest(new Response { Messege = "Fail", Data = null });
            return Ok(new Response { Messege = "Success", Data = listRoom });
        }
    }
    public class TaiKhoangDTO
    {
        public string  TenDayDu { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string QuocTich { get; set; }
        public string LoaiGiayTo { get; set; }
        public string SoGiayTo { get;set; }
        public string NgaySinh { get; set; }
    }
}
