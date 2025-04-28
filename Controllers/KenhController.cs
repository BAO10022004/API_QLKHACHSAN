using API_QLKHACHSAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/kenh")]
    [ApiController]
    public class KenhController : Controller
    {
        private readonly QuanLyKhachSanContext dbContext;
        public KenhController(QuanLyKhachSanContext _dbContext)
        {
            this.dbContext = _dbContext ?? new QuanLyKhachSanContext();
        }
        [HttpGet("LayKenh")]
        public IActionResult LayKenh()
        {
            List<KenhDatPhong> ListKenh = new List<KenhDatPhong>();
            try
            {
                ListKenh = dbContext.KenhDatPhongs.Where(p => p.TrangThai == true).ToList();

            }
            catch (Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
            return Ok(new Response()
            {
                Messege = "Success",
                Data = ListKenh
            });
        }
    }
}
