using DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DB.Models;
namespace WebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;

        public UserController(DBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        //Comment
        //[HttpPost]
        //public IActionResult Login([FromBody] LoginDto model)
        //{
        //    var user = _context.Users.SingleOrDefault(x => x.UserName == model.Username && x.Password == model.Password);
        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var claims = new[]
        //    {
        //           new System.Security.Claims.Claim("Id", user.Id),
        //           new Claim("UserName", user.UserName)
        //       };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var expires = DateTime.Now.AddDays(1);

        //    var token = new JwtSecurityToken(
        //        _configuration["Jwt:Issuer"],
        //        _configuration["Jwt:Issuer"],
        //        claims,
        //        expires: expires,
        //        signingCredentials: creds
        //    );

        //    return Ok(new
        //    {
        //        token = new JwtSecurityTokenHandler().WriteToken(token),
        //        expiration = expires
        //    });
        //}

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto model)
        {
            if (model.Username == null || model.Password == null)
            {
                return BadRequest("Invalid model");
            }

            var user = _context.Users.FirstOrDefault(x => x.UserName == model.Username && x.Password == model.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expires
            });
        }
    }

}