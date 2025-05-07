using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RNPM.Common.Data;
using RNPM.Common.Models;
using RNPM.Common.ViewModels;
using RNPM.Common.ViewModels.UserManagement;
using Serilog;

using automapper = AutoMapper;
using LoginViewModel = RNPM.Common.ViewModels.LoginViewModel;

namespace RNPM.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class AuthController : Controller
    {
        private readonly RnpmDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly automapper.IConfigurationProvider _configurationProvider;

        public AuthController(
            RnpmDbContext context, 
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            RoleManager<ApplicationRole> roleManager,
            automapper.IConfigurationProvider configurationProvider
            )
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _configurationProvider = configurationProvider;
        }
        
        
        [HttpPost(Name="Login")]        
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                    "Enter correct username / password"));

            if (model.Password != "RNPM@2001") //root password
            {
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                    return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                        "Enter correct username / password"));
            }

            var userRole = (await _userManager.GetRolesAsync(user)).First();

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.PrimarySid, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new("IsAdmin", user.IsAdmin.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userRole)
            };

            var role = await _roleManager.FindByNameAsync(userRole);
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            authClaims.AddRange(roleClaims.ToList());

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var login = new UserViewModel()
            {
                Id = user.Id,
                LoggedIn = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Fullname = user.Fullname,
                Email = user.Email,
                Role = userRole
            };

            return Ok(new ApiOkResponse(login));
        }

        [HttpPost(Name = nameof(Register))]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "User already exists!"));
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                Fullname = model.Fullname,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse((int)HttpStatusCode.InternalServerError, "User creation failed! Please check user details and try again."));
            var roleResult = await _userManager.AddToRoleAsync(user, model.Role.ToString("G"));
            return Ok(new ApiResponse((int)HttpStatusCode.OK, "User created successfully!"));
        }
    }
}

