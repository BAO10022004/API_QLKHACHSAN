using API_QLKHACHSAN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/Phong")]
    [ApiController]
    public class PhongController : Controller
    {
        readonly private QuanLyKhachSanContext dbContext;
        public PhongController(QuanLyKhachSanContext _dbContext)
        {
            this.dbContext = _dbContext ?? new QuanLyKhachSanContext();
        }

        [HttpGet("GetRoom")]
        [Authorize]
        public IActionResult GetRoom()
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
            var listRoom = dbContext.Phongs.ToList();
            if (listRoom == null)
                return BadRequest(new Response { Messesge = "Fail", ListRoom = null });
            return Ok(new Response { Messesge = "Success", ListRoom = listRoom });
        }

        [HttpPut("GetRoomEmpty")]
        [Authorize]
        public IActionResult GetRoomEmpty(DateTime? NgayNhan, DateTime? NgayTra)
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
            var listRoom = dbContext.Phongs.ToList();
            if (listRoom == null)
                return BadRequest(new Response { Messesge = "Fail", ListRoom = null });
            var listRoomEmpty = new List<Phong>();
            var listOrder = dbContext.DatPhongs.ToList();
            listRoom.ForEach(lr =>
            {
                if (lr.TrangThai == "1")
                {
                    if (!listOrder.Select(x => x.MaPhong).ToList().Contains(lr.MaPhong))
                        listRoomEmpty.Add(lr);
                    else
                    {
                        listOrder.ForEach(lo =>
                        {
                            if (lo.MaPhong.Equals(lr.MaPhong))
                            {
                                if (lo.NgayNhanPhong > (NgayNhan) &
                                     lo.NgayTraPhong < (NgayTra))
                                {
                                    listRoomEmpty.Add(lr);
                                }
                                else if (lo.TrangThaiDatPhong == "Cancel")
                                {
                                    listRoomEmpty.Add(lr);
                                }
                            }
                        });
                    }
                }
            });
            return Ok(new Response { Messesge = "Success", ListRoom = listRoom });
        }
    }

    public class Response
    {
        string messesge;
        List<Phong> listRoom;

        public string Messesge { get => messesge; set => messesge = value; }
        public List<Phong> ListRoom { get => listRoom; set => listRoom = value; }
    }


}
