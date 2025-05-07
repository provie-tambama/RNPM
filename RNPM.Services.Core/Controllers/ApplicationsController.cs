using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RNPM.Common.Data;
using RNPM.Common.Models;
using RNPM.Common.ViewModels.Core;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace RNPM.API.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class ApplicationsController : Controller
{
    private readonly RnpmDbContext _context;
    private readonly AutoMapper.IConfigurationProvider _configuration;

    public ApplicationsController(RnpmDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [HttpPost(Name = nameof(CreateApplication))]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationViewModel model)
    {
        var applicationExists = await _context.Applications.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.Name == model.Name && d.UserId == model.UserId).ConfigureAwait(false);
        if (applicationExists != null)
        {
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"Application {model.Name} already exists!"));
        }
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{model.Name}{model.UserId}"));
        string accessCode;
        using (var md5 = MD5.Create())
        {
            byte[] md5Hash = md5.ComputeHash(hashBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in md5Hash)
            {
                sb.Append(b.ToString("X2"));
            }

            accessCode = sb.ToString();
        }

        if (accessCode.Length < 3)
        {
            return BadRequest("Failed to process Access Code");
        }
        
        Application application = new Application()
        {
            Name = model.Name,
            UserId = model.UserId,
            UniqueAccessCode = accessCode
        };
        await _context.AddAsync(application).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        return Ok(new ApiOkResponse(application, "Application successfully created"));
    }
    
    [HttpGet(Name = nameof(GetApplications))]
    public async Task<IActionResult> GetApplications()
    {
        var applications = await _context.Applications
            .Where(d => d.IsActive && !d.IsDeleted)
            .ProjectTo<ApplicationViewModel>(_configuration)
            .ToListAsync();

        return Ok(applications.OrderByDescending(d => d.CreatedDate).ToList());
    }
    
    [HttpGet("{userId}", Name = nameof(GetUserApplications))]
    public async Task<IActionResult> GetUserApplications(string userId)
    {
        var applications = await _context.Applications
            .Where(d => d.UserId == userId && d.IsActive)
            .ProjectTo<ApplicationViewModel>(_configuration)
            .ToListAsync();

        return Ok(applications.OrderByDescending(d => d.CreatedDate).ToList());
    }
    [HttpGet("{id}", Name = nameof(GetApplication))]
    public async Task<IActionResult> GetApplication(string id)
    {
        var application = await _context.Applications.Include(a => a.Navigations )
            .Include(a => a.ScreenComponents)
            .Include(a => a.NetworkRequests)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (application == null)
        {
            return BadRequest("Application not found");
        }
        return Ok(application);
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteApplicationById))]
    public async Task<IActionResult> DeleteApplicationById(string id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        var application = await _context.Applications.FirstOrDefaultAsync(d => d.Id == id);

        if (application == null) return NotFound();
        
        _context.Applications.Remove(application);
        

        var navigations = await _context.Navigations.Where(n => n.ApplicationId == application.Id).ToListAsync();

        foreach (var navItem in navigations)
        {
            _context.Navigations.Remove(navItem);
        }
        
        var screenComponents = await _context.ScreenComponents.Where(n => n.ApplicationId == application.Id).ToListAsync();
        
        foreach (var i in screenComponents)
        {
            _context.ScreenComponents.Remove(i);
        }
        
        var httpRequests = await _context.NetworkRequests.Where(n => n.ApplicationId == application.Id).ToListAsync();
        
        foreach (var i in httpRequests)
        {
            _context.NetworkRequests.Remove(i);
        }
        
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new ApiOkResponse(application, $"Application {application.Name} was deleted successfully"));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ApplicationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }
    private bool ApplicationExists(string id)
    {
        return _context.Applications.Any(e => e.Id == id);
    }
}