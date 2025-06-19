using System.Net;
using System.Net.Mime;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RNPM.Common.Data;
using RNPM.Common.Data.Enums;
using RNPM.Common.Models;
using RNPM.Common.ViewModels.Core;
using Serilog;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace RNPM.API.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class NavigationsController : Controller
{
    private readonly RnpmDbContext _context;
    private readonly AutoMapper.IConfigurationProvider _configuration;

    public NavigationsController(RnpmDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [HttpPost(Name = nameof(CreateNavigationMetric))]
    public async Task<IActionResult> CreateNavigationMetric([FromBody] CreateNavigationViewModel model)
    {
        var application = await _context.Applications.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.UniqueAccessCode == model.UniqueAccessCode).ConfigureAwait(false);
        if (application == null)
        {
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"No Application with unique access code!"));
        }
        var navigation = await _context.Navigations.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.FromScreen == model.FromScreen && d.ToScreen == model.ToScreen && d.ApplicationId == application.Id).ConfigureAwait(false);
        
        DeviceInfo deviceInfo = null;
        if (model.DeviceInfo != null)
        {
            // Create a new device info record
            deviceInfo = new DeviceInfo
            {
                // Map properties from viewmodel
                Brand = model.DeviceInfo.Brand,
                ModelName = model.DeviceInfo.ModelName,
                Manufacturer = model.DeviceInfo.Manufacturer,
                DeviceName = model.DeviceInfo.DeviceName,
                Os = model.DeviceInfo.Os,
                OsVersion = model.DeviceInfo.OsVersion,
                OsName = model.DeviceInfo.OsName,
                OsBuildId = model.DeviceInfo.OsBuildId,
                DeviceType = model.DeviceInfo.DeviceType,
                DeviceYearClass = model.DeviceInfo.DeviceYearClass,
                IsDevice = model.DeviceInfo.IsDevice,
                SupportedCpuArchitectures = model.DeviceInfo.SupportedCpuArchitectures,
                TotalMemory = model.DeviceInfo.TotalMemory
            };
            
            await _context.DeviceInfos.AddAsync(deviceInfo);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        
        NavigationInstance metric;
        if (navigation == null)
        {
           var navigation1 = new Navigation()
            {
                ApplicationId = application.Id,
                FromScreen = model.FromScreen,
                ToScreen = model.ToScreen
            };
            await _context.AddAsync(navigation1).ConfigureAwait(false);
            metric = new NavigationInstance()
            {
                NavigationId = navigation1.Id,
                Date = DateTime.Now,
                NavigationCompletionTime = model.NavigationCompletionTime,
                DeviceInfoId = deviceInfo?.Id 
            };
        }
        else
        {
            metric = new NavigationInstance()
            {
                NavigationId = navigation.Id,
                Date = DateTime.Now,
                NavigationCompletionTime = model.NavigationCompletionTime,
                DeviceInfoId = deviceInfo?.Id 
            };
        }
        await _context.AddAsync(metric).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        return Ok(new ApiOkResponse(metric, "Metric successfully created"));
    }
    
    [HttpGet("{applicationId}",Name = nameof(GetNavigations))]
    public async Task<IActionResult> GetNavigations(string applicationId)
    {
        var items = await _context.Navigations
            .Where(d => d.IsActive && !d.IsDeleted && d.ApplicationId == applicationId)
            .ToListAsync();

        return Ok(items.OrderByDescending(d => d.CreatedDate).ToList());
    }

    [HttpGet("{navigationId}", Name = nameof(GetNavigation))]
    public async Task<IActionResult> GetNavigation(string navigationId)
    {
        var navigation = await _context.Navigations.Include(c => c.NavigationInstances)
            .ThenInclude(ni => ni.DeviceInfo).FirstOrDefaultAsync(c => c.Id == navigationId);
        
        if (navigation == null)
        {
            return NotFound("Navigation not found");
        }
    
        return Ok(navigation);
    }
    
    [HttpGet("{navigationId}", Name = nameof(GetNavigationAverages))]
    public async Task<IActionResult> GetNavigationAverages(string navigationId)
    {
        var navigation = await _context.Navigations.FindAsync(navigationId);
        if (navigation == null)
        {
            return NotFound("Navigation not found");
        }

        var instances = await _context.NavigationInstances
            .Include(i => i.DeviceInfo)
            .Where(i => i.NavigationId == navigationId && i.IsActive && !i.IsDeleted)
            .OrderByDescending(i => i.Date)
            .ToListAsync();

        var startDate = DateTime.Today.AddDays(-14); // Last 15 days including today
        var averages = new List<DailyAverageViewModel>();

        // Group by device type for more detailed analysis
        var instancesByDevice = instances
            .Where(i => i.Date >= startDate)
            .GroupBy(i => i.DeviceInfo != null ? i.DeviceInfo.DeviceType : "unknown");

        // Process each day's data
        for (int i = 0; i <= 14; i++)
        {
            var date = startDate.AddDays(i);
            var dailyInstances = instances.Where(inst => inst.Date.Date == date.Date).ToList();

            var average = new DailyAverageViewModel
            {
                Date = date,
                Average = (decimal)(dailyInstances.Any()
                    ? dailyInstances.Average(d => (double)d.NavigationCompletionTime)
                    : 0),
                Count = dailyInstances.Count
            };

            // Add device breakdown if available
            if (dailyInstances.Any(d => d.DeviceInfo != null))
            {
                average.DeviceBreakdown = dailyInstances
                    .GroupBy(d => d.DeviceInfo?.DeviceType ?? "unknown")
                    .Select(g => new DeviceAverageViewModel
                    {
                        DeviceType = g.Key,
                        Average = g.Average(d => (double)d.NavigationCompletionTime),
                        Count = g.Count()
                    })
                    .ToList();
            }

            averages.Add(average);
        }

        return Ok(averages);
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteNavigationById))]
    public async Task<IActionResult> DeleteNavigationById(string id)
    {
        var navigation = await _context.Navigations.FirstOrDefaultAsync(n => n.Id == id);
        if (navigation == null)
        {
            return BadRequest("Navigation doesn't exist");
        }

        var items = await _context.NavigationInstances.Where(i => i.NavigationId == id).ToListAsync();
        
        foreach (var i in items)
        {
            _context.NavigationInstances.Remove(i);
        }

        _context.Navigations.Remove(navigation);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new ApiOkResponse(navigation, $"Navigation from {navigation.FromScreen} to {navigation.ToScreen} was deleted successfully"));
        }
        catch (DbUpdateConcurrencyException)
        {
            {
                throw;
            }
        }
    }
}