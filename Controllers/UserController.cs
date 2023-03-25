using bugtrackerback.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace bugtrackerback.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, 
            SignInManager<User> signInManager, IConfiguration 
            configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegisterDTO registerData)
        {
            User userCheck = await _userManager.FindByEmailAsync(registerData.Email);

            if(userCheck != null)
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO loginData)
        {
            User loginUser = await _userManager.FindByEmailAsync(loginData.Email);
            var result = await _signInManager.PasswordSignInAsync(loginUser, loginData.Password, false, false);

            if(!result.Succeeded)
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

        [HttpPost("createRole")]
        public async Task<IdentityResult> CreateRole(string roleName)
        {
            var role = new IdentityRole { Name = roleName };

            // Create the role using the RoleManager
            var result = await _roleManager.CreateAsync(role);

            return result;
        }

        [HttpPost("Role")]
        public async Task<IActionResult> AddRole(string email, string roleName)
        {
            User user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }
    }
}
