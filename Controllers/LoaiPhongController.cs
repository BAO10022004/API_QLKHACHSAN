using API_QLKHACHSAN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/LoaiPhong")]
    [ApiController]
    public class LoaiPhongController : Controller
    {
        readonly private QuanLyKhachSanContext dbContext;
        API_QLKHACHSAN.Service.PhongServices PhongServices;
        public LoaiPhongController(QuanLyKhachSanContext _dbContext)
        {
            this.dbContext = _dbContext ?? new QuanLyKhachSanContext();
            PhongServices = new Service.PhongServices();
        }
        [HttpGet("TinhGiaLoaiPhong")]
        public IActionResult TinhGiaLoaiPhong(int maLoaiPhong, bool coThemNguoi)
        {
            var loaiPhong = dbContext.LoaiPhongs.FirstOrDefault(lp => lp.MaLoaiPhong == maLoaiPhong);
            if (loaiPhong == null)
            {
                return BadRequest("LoaiPhong not exits");
            }
            decimal tongTien = 0;
            tongTien = decimal.Parse(((loaiPhong.GiaCoBan + (coThemNguoi ? loaiPhong.PhiNguoiThem : 0))
                        * (1 - loaiPhong.PhanTramGiamGiaMacDinh / 100)
                        * (1 - loaiPhong.GiamGiaLuuTruDai / 100)
                        * (1 - loaiPhong.GiamGiaDatSom / 100).Value) + "");
            return Ok(tongTien);
        }
        [HttpGet("LayLoaiPhong")]
        public IActionResult LayLoaiPhong()
        {
            List<LoaiPhong> listLoaiPhong = new List<LoaiPhong>();
            try
            {
                listLoaiPhong = dbContext.LoaiPhongs.Where(p => p.TrangThai == true).ToList();

            }
            catch (Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
            return Ok(new Response()
            {
                Messenge = "Success",
                Data = listLoaiPhong
            });
        }
    }
}
