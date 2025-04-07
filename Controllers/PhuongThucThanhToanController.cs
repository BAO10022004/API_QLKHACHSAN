using API_QLKHACHSAN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/PhuongThucThanhToan")]
    [ApiController]
    public class PhuongThucThanhToanController : Controller
    {
        readonly API_QLKHACHSAN.Models.QuanLyKhachSanContext dbContext;
        public PhuongThucThanhToanController(QuanLyKhachSanContext _dbContext)
        {
            dbContext = _dbContext ?? new QuanLyKhachSanContext();
        }
        [HttpGet("LayRaPhuongThucThanhToan")]
        [Authorize]
        public ActionResult LayRaPhuongThucThanhToan()
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
            List<PhuongThucThanhToan> listPTTT = new List<PhuongThucThanhToan>();
            try
            {
                listPTTT = dbContext.PhuongThucThanhToans.Where(p => p.TrangThai == true).ToList();
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR : " + ex.Message);
            }
            return Ok(new Response(){ Messenge = "Get success", Data = listPTTT });
        }
    }
}
