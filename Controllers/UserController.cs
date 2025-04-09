using API_QLKHACHSAN.Models;
using com.sun.org.apache.bcel.@internal.generic;
using com.sun.tracing;
using com.sun.xml.@internal.ws.api;
using java.security.spec;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace API_QLKHACHSAN.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        API_QLKHACHSAN.Models.QuanLyKhachSanContext dbContext;
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration _configuration, QuanLyKhachSanContext _dbContext)
        {
            this.dbContext = _dbContext ?? throw new ArgumentNullException(nameof(_dbContext));
            this._configuration = _configuration;
        }

        [HttpPost("SignUpUser")]
        public IActionResult SignUpUser(RequestSignUp requestSignUp)
        {            // check username
            if (requestSignUp.username.Equals("") ||
               requestSignUp.password.Equals("") ||
               requestSignUp.email.Equals("")
               )
            {
                return BadRequest("Missing information");
            }
            if (requestSignUp.username.Length < 8 || requestSignUp.password.Length < 8)
            {
                return BadRequest("Username and password must be at least 8 characters");
            }
            try
            {
                if (dbContext.Users.FirstOrDefault(x => x.Username == requestSignUp.username) != null)
                {
                    return BadRequest("Username is already taken");
                }
            }
            catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
            // check password
            try
            {
                if (!Regex.IsMatch(requestSignUp.password, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!.#$@_+,?-]).{8,50}$"))
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

                if (!Regex.IsMatch(requestSignUp.email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
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
                if (dbContext.Users.FirstOrDefault(x => x.Username == requestSignUp.email) != null)
                {
                    return BadRequest("Email is already taken");
                }
            }
            catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }

            // Add user
            API_QLKHACHSAN.Models.User user = new API_QLKHACHSAN.Models.User();
            user.Username = requestSignUp.username;
            try
            {
                var salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }
                using (var pbkdf2 = new Rfc2898DeriveBytes(requestSignUp.password, salt, 65536, HashAlgorithmName.SHA1))
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
            user.Email = requestSignUp.email;
            user.DateCreated = DateTime.Now;
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                transaction.Commit();
                return Ok(new Response() { 
                    Messenge = "Success",
                    Data = user 
                });
            }
            catch (DbUpdateException ex)
            {
                transaction.Rollback();
                return BadRequest($"Database error: {ex.Message}");
            }
        }

        [HttpPut("SignIn")]
        public IActionResult SignInUser(RequestSignIn requestSignIn)
        {
            if (string.IsNullOrWhiteSpace(requestSignIn.password) ||
                (string.IsNullOrWhiteSpace(requestSignIn.username) && string.IsNullOrWhiteSpace(requestSignIn.email)))
            {
                return BadRequest(new
                {
                    errorCode = "INVALID_INPUT",
                    message = "Username/email and password are required."
                });
            }

            try
            {
                var user = dbContext.Users
                    .FirstOrDefault(x => x.Username == requestSignIn.username || x.Email == requestSignIn.email);

                if (user == null)
                {
                    return Unauthorized(new
                    {
                        errorCode = "USER_NOT_FOUND",
                        message = "Username or email is incorrect."
                    });
                }

                if (!VerifyPassword(requestSignIn.password, user.PasswordHash))
                {
                    return Unauthorized(new
                    {
                        errorCode = "INVALID_PASSWORD",
                        message = "Password is incorrect."
                    });
                }

                var token = GenerateJwtToken(user);
                Session.Token = token;

                return Ok(new
                {
                    message = "Success",
                    data = new
                    {
                        token = token,
                        user = user
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    errorCode = "SERVER_ERROR",
                    message = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }

        private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(storedPasswordHash);
                var salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                using var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, 65536, HashAlgorithmName.SHA1);
                var inputHash = pbkdf2.GetBytes(16);

                for (int i = 0; i < 16; i++)
                {
                    if (hashBytes[i + 16] != inputHash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        [HttpGet("GetUserByEmail")]
        public IActionResult GetUserByEmail(string email)
        {
            
            if (email.Equals(""))
            {
                return BadRequest("Fail: email is empty");
            }
            var user = dbContext.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            return Ok(new Response() {Messenge = "Success", Data = user });
        }
        [HttpGet]
        public string GenerateJwtToken(User user)
        {

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"] ?? throw new ArgumentNullException("AppSettings:SecretKey")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["AppSettings:Issuer"],
                audience: _configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpGet("AddRoleForUser")]
        public IActionResult AddRoleForUser(string? username, string? email, List<string> roleNameList)
        {
            // vetify data
            if (username == null && email == null)
                return BadRequest("Must input username or email");
            if (roleNameList == null)
                return BadRequest("Must input role");
            //Find user
            var user = dbContext.Users.FirstOrDefault(x => x.Username.Equals(username) || x.Email.Equals(email));
            if (user == null)
            {
                return NotFound("Not found user");
            }
            //Find role
            List<Role> roleList = new List<Role>();
            roleNameList.ForEach(name =>
            {
                roleList.Add(dbContext.Roles.FirstOrDefault(r => r.RoleName == name));
            });
            if (roleList.Count == 0)
                return NotFound("Not found role");
            // Set up
            List<UserRole> roleUserList = new List<UserRole>();
            roleList.ForEach(x =>
            {
                var userRole = new UserRole();
                userRole.UserId = user.UserId;
                userRole.RoleId = x.RoleId;
            });
            //Save
            dbContext.UserRoles.AddRange(roleUserList);
            dbContext.SaveChanges();
            return Ok(new Response() {Messenge= "Add role success",Data= user});
        }
    }
    public class RequestSignUp
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }

    }
    public class RequestSignIn
    {
        public string? username { get; set; }
        public string? email { get; set; }
        public string password { get; set; }
    }


}
