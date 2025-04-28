using API_QLKHACHSAN.Models;
using API_QLKHACHSAN.Service;
using com.sun.corba.se.impl.protocol.giopmsgheaders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/DatPhong")]
    [ApiController]
    public class DatPhongController : Controller
    {
        readonly private QuanLyKhachSanContext dbContext;
        API_QLKHACHSAN.Service.PhongServices PhongServices;
        public DatPhongController(QuanLyKhachSanContext _dbContext)
        {
            this.dbContext = _dbContext ?? new QuanLyKhachSanContext();
        }
        [HttpGet("LayDatPhong")]
        public IActionResult LayDatPhong()
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
            List<DatPhong> listDatPhong = new List<DatPhong>();
            try
            {
                listDatPhong = dbContext.DatPhongs.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
            return Ok(new { Message = "Success", Data = listDatPhong });
        }
        [HttpPut("DatPhong")]
        public async Task<IActionResult> DatPhong(DatPhongDTO requestOrder)
        {
            PhongServices = new PhongServices();
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == currentUsername);
            if (user == null)
                return BadRequest("Must sign in to countinue");
            var roles = dbContext.UserRoles.Where(u => u.UserId == user.UserId).ToList().Select(u => u.RoleId);
            // Check Vetify
            if (!roles.Contains(2) && !(roles.Contains(3)) && !(roles.Contains(7)))
            {
                return StatusCode(403, "You do not have permission to get room.");
            }
            if (user == null)
            {
                return Unauthorized("Must sign in to continue");
            }
            if (requestOrder == null)
            {
                return BadRequest("Must enter properties order");
            }


            var phong = await dbContext.Phongs.FirstOrDefaultAsync(p => p.SoPhong.Equals(requestOrder.SoPhong + ""));
            if (phong == null)
            {
                return BadRequest("Room number is invalid");
            }

            var kenh = await dbContext.KenhDatPhongs.FirstOrDefaultAsync(
                k => k.MaKenh.Equals(requestOrder.MaKenh));
            if (kenh == null)
            {
                return BadRequest("Booking channel is invalid");
            }

            if (!TryParseDates(requestOrder, out DateTime ngayNhanPhong, out DateTime ngayTraPhong, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            if (!DateTime.TryParse(requestOrder.NgayDat, out DateTime ngayDat) || ngayDat > DateTime.Now)
            {
                return BadRequest("Booking date is invalid");
            }
            List<Phong> availableRooms;
            try
            {
                availableRooms = await PhongServices.GetListRoom();
            }
            catch (Exception e)
            {
                return BadRequest("Errorr access API");
            }

            if (!availableRooms.Any(x => x.MaPhong == phong.MaPhong))
            {
                return BadRequest("Room is already booked for the selected dates");
            }

            var loaiPhong = await dbContext.LoaiPhongs.FirstOrDefaultAsync(p => p.MaLoaiPhong == phong.MaLoaiPhong);
            if (loaiPhong == null)
            {
                return BadRequest("Room type not found");
            }
            if (!string.IsNullOrEmpty(requestOrder.TuoiTreEm) && int.TryParse(requestOrder.TuoiTreEm, out int tuoiTreEm)
                && tuoiTreEm > loaiPhong.TuoiToiDaTreEm)
            {
                return BadRequest("Child age exceeds the limit for this room type");
            }
            var khachHang = dbContext.KhachHangs.FirstOrDefault(x => x.SoDienThoai.Equals(requestOrder.SDTKhachHang));

            if(khachHang == null)
            {
                return BadRequest("Khach Hang not exits");
            }
            DatPhong order = new DatPhong
            {
                MaDatPhongHienThi = requestOrder.SoPhong,
                MaKhachHang = khachHang.MaKhachHang,
                MaPhong = phong.MaPhong,
                MaKenh = kenh.MaKenh,
                NgayNhanPhong = ngayNhanPhong,
                NgayTraPhong = ngayTraPhong,
                SoNguoiLon = requestOrder.SoNguoiLon,
                SoTreEm = requestOrder.SoTreEm,
                TuoiTreEm = requestOrder.TuoiTreEm,
                NgayDat = ngayDat,
                TrangThaiDatPhong = "PENDING",
                YeuCauDacBiet = requestOrder.YeuCauDatBiet,
                SoGiuongPhu = requestOrder.SoGiuongPhu,
                CoAnSang = requestOrder.CoAnSang,
                PhanTramGiamGia = requestOrder.PhanTramGiamGia,
                LyDoGiamGia = requestOrder.LyDoGiamGia,
                MaNhanVienDat = dbContext.NhanViens.FirstOrDefault(x => x.User.UserId == user.UserId).UserId

            };

            bool exceedsCapacity = (order.SoNguoiLon > loaiPhong.SoNguoiToiDa) ||
                                   (order.SoTreEm > loaiPhong.SoTreEmToiDa);

            LoaiPhongServices loaiPhongServices = new LoaiPhongServices();
            order.TongTien = await loaiPhongServices.TinhGiaLoaiPhong(loaiPhong.MaLoaiPhong, exceedsCapacity);

            if (order.PhanTramGiamGia > 0)
            {
                order.TongTien *= (1 - order.PhanTramGiamGia / 100);
            }
            order.TongTien *= (1 - dbContext.LoaiKhachHangs.FirstOrDefault(x => x.MaLoaiKhach == khachHang.MaKhachHang).PhanTramGiamGia / 100);
            await dbContext.DatPhongs.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return Ok(new { Message = "Room booked successfully", BookingCode = order.MaDatPhongHienThi });
        }
        private string GenerateBookingCode()
        {
            DateTime now = DateTime.Now;
            int bookingsToday = dbContext.DatPhongs.Count(dp => dp.NgayDat == now.Date);
            return $"{now.Day}{now.Month}{now.Year}{bookingsToday}";
        }

        private bool TryParseDates(DatPhongDTO request, out DateTime checkIn, out DateTime checkOut, out string errorMessage)
        {
            errorMessage = null;
            checkIn = default;
            checkOut = default; 

            try
            {
                checkIn = DateTime.Parse(request.NgayNhanPhong);
            }
            catch
            {
                errorMessage = "Check-in date is invalid";
                return false;
            }

            try
            {
                checkOut = DateTime.Parse(request.NgayTraPhong);
            }
            catch
            {
                errorMessage = "Check-out date is invalid";
                return false;
            }

            if (checkIn > checkOut)
            {
                errorMessage = "Check-in date cannot be after check-out date";
                return false;
            }

            // Optional: Check if check-in date is not in the past
            // if (checkIn < DateOnly.FromDateTime(DateTime.Now))
            // {
            //     errorMessage = "Check-in date cannot be in the past";
            //     return false;
            // }

            return true;
        }
        [HttpPut("CancelBill")]
        [Authorize]
        public async Task<IActionResult> HuyDatPhongAsync(RequestCancelBill requestCancelBill)
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == currentUsername);
            if (user == null)
                return BadRequest("Must sign in to countinue");
            var roles = dbContext.UserRoles.Where(u => u.UserId == user.UserId).ToList().Select(u => u.RoleId);
            // Check Vetify
            if (!roles.Contains(2) && !(roles.Contains(3)) && !(roles.Contains(7)))
            {
                return StatusCode(403, "You do not have permission to get room.");
            }
            if (requestCancelBill == null)
            {
                return BadRequest("Must enter properties order");
            }
            var order = dbContext.DatPhongs.FirstOrDefault(dp => dp.MaDatPhongHienThi.Equals(dp.MaDatPhongHienThi));
            if (order == null)
            {
                return NotFound("Not Found order");
            }
            order.CoTinhPhiHuy = requestCancelBill.CoTinhPhiHuy;
            order.LyDoHuy = requestCancelBill.LyDoHuy;
            order.TrangThaiDatPhong = "CANCEL";
            //order.TongTien = 
            dbContext.DatPhongs.Update(order);
            return Ok("HuyDatPhong successfully");
        }

        [HttpPut("ApproveBill")]
        [Authorize]
        public async Task<ActionResult> XacNhanDatPhongAsync(String MaDatPhong)
        {
            // Get role
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == currentUsername);
            if (user == null)
                return BadRequest("Must sign in to countinue");
            var roles = dbContext.UserRoles.Where(u => u.UserId == user.UserId).ToList().Select(u => u.RoleId);
            // Check Vetify
            if (!roles.Contains(2) && !(roles.Contains(3)) && !(roles.Contains(7)))
            {
                return StatusCode(403, "You do not have permission to get room.");
            }

            var order = dbContext.DatPhongs.FirstOrDefault( x=> x.MaDatPhongHienThi == MaDatPhong);
            if (order == null)
                return BadRequest("MaDatPhong not exits");
            order.TrangThaiDatPhong = "APPROVE";
            dbContext.DatPhongs.Update(order);
            dbContext.SaveChanges();
            return Ok(new { Message = "Approve success ", DatPhong = order });
        }
    
    }
    public class DatPhongDTO
    {
        public int? MaKenh { get; set; }
        public string NgayNhanPhong { get; set; }
        public string NgayTraPhong { get; set; }
        public int SoNguoiLon { get; set; }
        public int SoTreEm { get; set; }
        public string TuoiTreEm { get; set; }
        public string YeuCauDatBiet { get; set; }
        public int SoGiuongPhu { get; set; }
        public bool CoAnSang { get; set; }
        public decimal PhanTramGiamGia { get; set; }
        public string LyDoGiamGia { get; set; }
        public string? SDTKhachHang { get; set; }
        public string SoPhong { get; set; }
        public string NgayDat { get; set; }
    }
    public class RequestCancelBill
    {
        public int MaDatPhongHienThi { get => MaDatPhongHienThi; set => MaDatPhongHienThi = value; }
        public bool CoTinhPhiHuy { get => CoTinhPhiHuy; set => CoTinhPhiHuy = value; }
        public string LyDoHuy { get => LyDoHuy; set => LyDoHuy = value; }
    }

}
