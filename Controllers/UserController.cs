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
        [HttpPost]
        public IActionResult SignUpUser(RequestSignUp requestSignUp)
        {
            ApiResponse reponsibility = new ApiResponse();

            reponsibility.Status = "Fail";
            reponsibility.Message = "Sign up Fail";
            reponsibility.Result = null;
            // check username
            if (requestSignUp.username.Equals("") || 
               requestSignUp.password.Equals("") || 
               requestSignUp.email.Equals("") || 
               requestSignUp.role.Equals("")
               )
            {
                reponsibility.Message = "Missing information";
                return BadRequest(reponsibility);
            }
            if(requestSignUp.username.Length < 8 || requestSignUp.password.Length < 8)
            {
                reponsibility.Message = "Username and password must be at least 8 characters";
                return BadRequest(reponsibility);
            }
            try
            {
                if (dbContext.Users.FirstOrDefault(x=> x.Username == requestSignUp.username) != null)
                {
                    reponsibility.Message = "Username is already taken";
                    return BadRequest(reponsibility);
                }
            }catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
            // check password
            try
            {
                if (!Regex.IsMatch(requestSignUp.password, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!.#$@_+,?-]).{8,50}$"))
                {
                    reponsibility.Message = "Password is incorrect";
                    return BadRequest(reponsibility);
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
                    reponsibility.Message = "Email is incorrect";
                    return BadRequest(reponsibility);
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
                    reponsibility.Message = "Email is already taken";
                    return BadRequest(reponsibility);
                }
            }
            catch (System.Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
            // check role
            try {
                if (dbContext.Roles.FirstOrDefault(x => x.RoleName == requestSignUp.role) == null)
                {
                    reponsibility.Message = "Role is incorrect";
                    return BadRequest(reponsibility);
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
                reponsibility.Message = "Can't hash password";
                return BadRequest(reponsibility);
            }
            user.Email = requestSignUp.email;
            user.DateCreated = DateTime.Now;
           
            // add role 
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                UserRole userRole = new UserRole();
                var Role = dbContext.Roles.FirstOrDefault(x => x.RoleName == requestSignUp.role);
                try
                {
                    userRole.UserId = user.UserId;
                    userRole.RoleId = Role.RoleId;
                }
                catch (System.Exception e)
                {
                    reponsibility.Message = "Can't find role in database";
                    return BadRequest(reponsibility);
                }
                dbContext.UserRoles.Add(userRole);
                dbContext.SaveChanges();

                transaction.Commit();
                reponsibility.Status = "Success";
                return Ok(reponsibility);
            }
            catch (DbUpdateException ex)
            {
                transaction.Rollback();
                return BadRequest($"Database error: {ex.Message}");
            }
            return Ok(reponsibility);
        }
        [HttpGet("SignIn")]
        public IActionResult SignInUser(RequestSignIn requestSignIn)
        {
            if (string.IsNullOrEmpty(requestSignIn.username) && string.IsNullOrEmpty(requestSignIn.email) || string.IsNullOrEmpty(requestSignIn.password))
            {
                return BadRequest("Missing information");
            }
            try
            {
                var user = dbContext.Users
                            .FirstOrDefault(x => x.Username == requestSignIn.username || x.Email == requestSignIn.email);

                if (user == null)
                {
                    return BadRequest("Username is incorrect");
                }
                byte[] hashBytes = Convert.FromBase64String(user.PasswordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                using (var pbkdf2 = new Rfc2898DeriveBytes(requestSignIn.password, salt, 65536, HashAlgorithmName.SHA1))
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
            ApiResponse response = new ApiResponse();
            response.Status = "Fail";
            response.Message = "User not found";
            response.Result = null;
            if (email.Equals(""))
            {
                return BadRequest("Fail: email is empty");
            }
            var user = dbContext.Users.FirstOrDefault(x => x.Email == email);
            if(user == null)
            {
                return BadRequest("User not found");
            }
            response.Status = "Success";
            response.Message = "User found";
            response.Result = user;
            return Ok(response);
        }

    }
    public class RequestSignUp
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string role { get; set; }
    }
    public class RequestSignIn
    { 
       public  string? username { get; set; }
       public  string? email { get; set; }
       public string password { get; set; }
    }
    public class Reponsibility {
        public string reponMessage { get; set; }
        public User user { get; set; }
    }


}
