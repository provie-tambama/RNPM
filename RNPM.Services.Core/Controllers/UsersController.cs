using System.Net;
using System.Net.Mime;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RNPM.API.Utilities.Extensions;
using RNPM.Common.Data;
using RNPM.Common.Models;
using RNPM.Common.ViewModels;
using RNPM.Common.ViewModels.UserManagement;
using Serilog;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Agrifora.API.Controllers
{
    [Route("api/[controller]/[action]")]
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    
    public class UsersController : ControllerBase
    {
        private readonly RnpmDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfigurationProvider _configuration;
        //private readonly ISmsService _smsService;
        //private readonly ISendGridEmailService _sendGridEmailService;
        //private readonly ISendGridMessageHelper _sendGridMessageHelper;
        //private readonly EmailSettings _emailSettings;

        public UsersController(
            RnpmDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfigurationProvider configuration
            //ISmsService smsService,
            //IOptions<EmailSettings> emailSettings,
            //ISendGridEmailService sendGridEmailService,
            //ISendGridMessageHelper sendGridMessageHelper
            )
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            //_smsService = smsService;
            //_emailSettings = emailSettings.Value;
            //_sendGridEmailService = sendGridEmailService;
            //_sendGridMessageHelper = sendGridMessageHelper;
        }

        // GET: api/users
        [HttpGet("", Name = nameof(GetUsers))]
        public async Task<IActionResult> GetUsers()
        {
            Log.Information("trying");
            var users = await _context.Users
                .Where(u => u.IsActive && !u.IsDeleted)
                .ProjectTo<UserViewModel>(_configuration)
                .ToListAsync();

            return Ok(users);
        }
        
        //Search locations by name
        [HttpGet("{search}", Name = nameof(GetUsersByName))]
    
        public async Task<IActionResult> GetUsersByName(string search)
        {
            var users = await _context.Users
                .Where(u => u.Fullname.Contains(search) && u.IsActive && !u.IsDeleted)
                .ProjectTo<UserViewModel>(_configuration)
                .ToListAsync()
                .ConfigureAwait(false);
            
            return Ok(users);
        }

        // GET api/users/5
        [HttpGet("{id}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.Users
                .ProjectTo<UserViewModel>(_configuration)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{username}", Name = nameof(GetUserByUsername))]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.Trim().ToLower())
                .ConfigureAwait(false);

            if (user == null) return NotFound();

            return Ok(user);
        }
        
        // POST api/users
        [HttpPost("", Name = nameof(DeleteUser))]
        public async Task<IActionResult> DeleteUser([FromBody] string id)
        {
            Log.Information("delete");
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "User does not exist!"));

            user.IsDeleted = true;
            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse((int)HttpStatusCode.InternalServerError, "User deletion failed! Please check user details and try again."));

            return Ok(new ApiOkResponse(null, "User deleted successfully!"));
        }

        // PUT api/users/5
        [HttpPut("{id}", Name = nameof(UpdateUser))]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserViewModel model)
        {
            // Check if the provided ID matches the model's ID
            if (id != model.Id)
            {
                return BadRequest("ID mismatch");
            }

            var user = await _context.Users.FirstOrDefaultAsync(l => l.Id == id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update user properties
            user.Fullname = model.Fullname;
            user.PhoneNumber = model.PhoneNumber.GetMsisdn("0");

            try
            {
                // Update user in the database
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Check if the role is provided in the request
                if (!string.IsNullOrEmpty(model.Role.ToString()))
                {
                    // Remove existing roles
                    var existingRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, existingRoles);

                    // Add the new role
                    var roleResult = await _userManager.AddToRoleAsync(user, model.Role.ToString("G"));

                    if (!roleResult.Succeeded)
                    {
                        return StatusCode(
                            StatusCodes.Status500InternalServerError,
                            new ApiResponse((int)HttpStatusCode.InternalServerError, "User role update failed! Please check user details and try again.")
                        );
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Concurrency conflict occurred");
            }

            return Ok(new ApiOkResponse(user, $"{user.Fullname} was updated successfully"));
        }
        
        [HttpPut("{id}", Name = nameof(UpdateMyProfile))]
        public async Task<IActionResult> UpdateMyProfile(string id, [FromBody] UpdateMyProfileViewModel model)
        {
            // Check if the provided ID matches the model's ID
            if (id != model.Id)
            {
                return BadRequest("ID mismatch");
            }

            var user = await _context.Users.FirstOrDefaultAsync(l => l.Id == id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update user properties
            user.Fullname = model.Fullname;
            user.PhoneNumber = model.PhoneNumber.GetMsisdn("0");

            try
            {
                // Update user in the database
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Concurrency conflict occurred");
            }

            return Ok(new ApiOkResponse(user, $"Your profile was updated successfully"));
        }
        
        
        [HttpPut("{id}", Name = nameof(ChangePassword))]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordViewModel model)
        {
            // Check if the provided ID matches the model's ID
            if (id != model.UserId)
            {
                return BadRequest("ID mismatch");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Verify the old password
            var passwordVerificationResult = await _userManager.CheckPasswordAsync(user, model.OldPassword);

            if (!passwordVerificationResult)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest,
                    "Invalid old Password!"));
            }

            // Change the password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse((int)HttpStatusCode.InternalServerError, "Password change failed! Please check your password and try again.")
                );
            }

            return Ok(new ApiOkResponse(null, "Password changed successfully"));
        }
        
        [HttpPut("{id}", Name = nameof(ChangePasswordAsAdmin))]
        public async Task<IActionResult> ChangePasswordAsAdmin(string id, [FromBody] ChangePasswordAsAdminViewModel model)
        {
            // Check if the provided ID matches the model's ID
            if (id != model.UserId)
            {
                return BadRequest("ID mismatch");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Change the password without checking the old password
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);

            if (!removePasswordResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse((int)HttpStatusCode.InternalServerError, "Password change failed! Please check your input and try again."));
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);

            if (!addPasswordResult.Succeeded)
            {
                // Log or handle the failure to add the new password
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse((int)HttpStatusCode.InternalServerError, "Password change failed! Please check your input and try again."));
            }

            return Ok(new ApiOkResponse(null, "Password changed successfully"));
        }
    }
}