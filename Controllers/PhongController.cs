using API_QLKHACHSAN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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
        public IActionResult GetRoom()
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
            var listRoom = dbContext.Phongs.ToList();
            if (listRoom == null)
                return BadRequest(new Response { Messege = "Fail", Data = null });
            return Ok(new Response { Messege = "Success", Data = listRoom });
        }

        [HttpPut("GetRoomEmpty")]
        public IActionResult GetRoomEmpty(String? NgayNhan, String? NgayTra)
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
            var listRoom = dbContext.Phongs.ToList();
            if (listRoom == null)
                return BadRequest(new Response { Messege = "Fail", Data = null });
            var listRoomEmpty = new List<Phong>();
            var listOrder = dbContext.DatPhongs.ToList();
            DateTime dateNgayNhan, dateNgayTra;
            if (
                !DateTime.TryParseExact(
                    NgayNhan,            // chuỗi cần parse
                    "dd-MM-yyyy",         // format cần đúng
                    CultureInfo.InvariantCulture, // culture dùng để parse (bắt buộc có)
                    DateTimeStyles.None,  // style option
                    out dateNgayNhan      // kết quả output
                )
             )
            {
                return BadRequest("NgayNhan incorrect");
            }
            if (
                !DateTime.TryParseExact(
                    NgayTra,            // chuỗi cần parse
                    "dd-MM-yyyy",         // format cần đúng
                    CultureInfo.InvariantCulture, // culture dùng để parse (bắt buộc có)
                    DateTimeStyles.None,  // style option
                    out dateNgayTra      // kết quả output
                )
             )
            {
                return BadRequest("NgayTra incorrect");
            }
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
                                if (lo.NgayNhanPhong > (dateNgayNhan) &
                                     lo.NgayTraPhong < (dateNgayTra))
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
            return Ok(new Response { Messege = "Success", Data = listRoom });
        }
    }

   

}
