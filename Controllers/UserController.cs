using API_QLKHACHSAN.Models;
using com.sun.org.apache.bcel.@internal.generic;
using java.security.spec;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        API_QLKHACHSAN.Models.QuanLyKhachSanContext dbContext = new API_QLKHACHSAN.Models.QuanLyKhachSanContext();
        [HttpGet]
        public IActionResult Index()
        {
            
            return Ok();
        }
        [HttpPost]
        public IActionResult SignUpUser(String username, String password, String email, String role)
        {
            // check username
            if(username.Equals("") || password.Equals("") || email.Equals("") || role.Equals(""))
            {
                return BadRequest("Missing information");
            }
            if(username.Length < 8 || password.Length < 8)
            {
                return BadRequest("Username and password must be at least 8 characters");
            }
            try
            {
                if (dbContext.Users.FirstOrDefault(x=> x.Username == username) != null)
                {
                    return BadRequest("Username is already taken");
                }
            }catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
            // check password
            try
            {
                if (!Regex.IsMatch(password, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!.#$@_+,?-]).{8,50}$"))
                {
                    return BadRequest("Password is incorrect");
                }
            }
            catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }

            // check email
            try
            {
               
                if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    return BadRequest("Email is incorrect");
                }
            }
            catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
            
            try
            {
                if (dbContext.Users.FirstOrDefault(x => x.Username == email) != null)
                {
                    return BadRequest("Email is already taken");
                }
            }
            catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
            // check role
            try {
                if (dbContext.Roles.FirstOrDefault(x => x.RoleName == role) == null)
                {
                    return BadRequest("Role is incorrect");
                }
            }
            catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
            // Add user
            API_QLKHACHSAN.Models.User user = new API_QLKHACHSAN.Models.User();
            user.Username = username;
            try
            {
                var salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 65536, HashAlgorithmName.SHA1))
                {
                    byte[] hash = pbkdf2.GetBytes(16); 

                    byte[] hashBytes = new byte[salt.Length + hash.Length];
                    Array.Copy(salt, 0, hashBytes, 0, salt.Length);
                    Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

                    user.PasswordHash = Convert.ToBase64String(hashBytes);
                }
            }
            catch (System.Exception e)
            {
                return BadRequest("Can't hash password");
            }
            user.Email = email;
            user.DateCreated = DateTime.Now;
           
            // add role 
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                UserRole userRole = new UserRole();
                var Role = dbContext.Roles.FirstOrDefault(x => x.RoleName == role);
                try
                {
                    userRole.UserId = user.UserId;
                    userRole.RoleId = Role.RoleId;
                }
                catch (System.Exception e)
                {
                    return BadRequest("Can't find user in database");
                }
                dbContext.UserRoles.Add(userRole);
                dbContext.SaveChanges();

                transaction.Commit();
                return Ok("User added successfully");
            }
            catch (DbUpdateException ex)
            {
                transaction.Rollback();
                return BadRequest($"Database error: {ex.Message}");
            }
            
            return Ok("Update");
        }
        [HttpGet("SignIn")]
        public IActionResult SignInUser([FromQuery] string? username, [FromQuery] string? email,[FromQuery] string password)

        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Missing information");
            }
            try
            {
                var user = dbContext.Users
                            .FirstOrDefault(x => x.Username == username || x.Email == email);

                if (user == null)
                {
                    return BadRequest("Username is incorrect");
                }
                byte[] hashBytes = Convert.FromBase64String(user.PasswordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 65536, HashAlgorithmName.SHA1))
                {
                    byte[] hash = pbkdf2.GetBytes(16);
                    for (int i = 0; i < 16; i++)
                    {
                        if (hashBytes[i + 16] != hash[i])
                        {
                            return BadRequest("Password is incorrect");
                        }
                    }
                }
                return Ok("Login successfully");
            }
            catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
        }
        [HttpGet("GetUserByEmail")]
        public IActionResult GetUserByEmail(string email)
        {
            if(email.Equals(""))
            {
                return BadRequest("Fail: email is empty");
            }
            var user = dbContext.Users.FirstOrDefault(x => x.Email == email);
            if(user == null)
            {
                return BadRequest("User not found");
            }
            return Ok(user);
        }
    }
}
