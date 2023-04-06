using bugtrackerback.Areas.Identity.Data;
using bugtrackerback.Entities;
using bugtrackerback.Entities.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace bugtrackerback.Controllers
{
    // [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly bugtrackerdbContext _context;

        public UserController(UserManager<User> userManager,
            SignInManager<User> signInManager, IConfiguration
            configuration, RoleManager<IdentityRole> roleManager,
            bugtrackerdbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(UserRegisterDTO registerData)
        {
            User userCheck = await _userManager.FindByEmailAsync(registerData.Email);

            if (userCheck != null)
            {
                return BadRequest("A user with this email already exists");
            }
            User registerUser = new User
            {
                Name = registerData.Name,
                Surname = registerData.Surname,
                Email = registerData.Email,
                PhoneNumber = registerData.PhoneNumber,
                UserName = registerData.Email
            };

            var result = await _userManager.CreateAsync(registerUser, registerData.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("User has been created");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO loginData)
        {
            User loginUser = await _userManager.FindByEmailAsync(loginData.Email);
            var result = await _signInManager.PasswordSignInAsync(loginUser, loginData.Password, false, false);

            if (!result.Succeeded)
            {
                return BadRequest("Invalid email or password");
            }

            var sessionToken = CreateToken(loginUser);

            return Ok(sessionToken);
        }



        private string CreateToken(User user)
        {
            var userRoles = _userManager.GetRolesAsync(user).GetAwaiter();
            // Claims describe the user that is authenticated
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
            };

            foreach (var userRole in userRoles.GetResult())
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding
                .UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            return jwt;
        }

        [HttpPost]
        public async Task<IdentityResult> CreateRole(string roleName)
        {
            var role = new IdentityRole { Name = roleName };

            // Create the role using the RoleManager
            var result = await _roleManager.CreateAsync(role);

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string email, string roleName)
        {
            User user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserData()
        {   
            var users = await _context.Users.Select(u => new { u.Id, u.Name, u.Surname, u.Email }).ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public Task<bool> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes
                (_configuration.GetSection("AppSettings:Token").Value)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        [HttpGet]
        public Task<string> GetUserEmail(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var email = token.Claims.First(c => c.Type == ClaimTypes.Name).Value;

            return Task.FromResult(email);
        }

        [HttpGet]
        public async Task<IActionResult> GetCreatedTickets(string userId)
        {
            var tickets = await _context.Tickets.Where(t => t.AuthorId == userId).ToListAsync();

            return Ok(tickets);
        }
    }
}
