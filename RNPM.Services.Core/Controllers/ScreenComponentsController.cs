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
using RNPM.Common.ViewModels.Update;
using Serilog;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace RNPM.Services.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class ScreenComponentsController : Controller
{
    private readonly RnpmDbContext _context;
    private readonly AutoMapper.IConfigurationProvider _configuration;

    public ScreenComponentsController(RnpmDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [HttpPost(Name = nameof(CreateComponentRenderMetric))]
    public async Task<IActionResult> CreateComponentRenderMetric([FromBody] CreateRenderComponentViewModel model)
    {
        var application = await _context.Applications.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.UniqueAccessCode == model.UniqueAccessCode).ConfigureAwait(false);
        if (application == null)
        {
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"No Application with unique access code!"));
        }
        var screenComponent = await _context.ScreenComponents.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.Name == model.Name && d.ApplicationId == application.Id).ConfigureAwait(false);
        
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
        }
        ScreenComponentRender metric;
        Random random = new Random();
        //if (model.RenderTime > 4000)
        //{
        //    var randomValue = random.NextDouble() * (1000.0 - 50.0) + 50.0;
        //    model.RenderTime = Math.Round((decimal)randomValue, 8); 
        //}

        if (screenComponent == null)
        {
           var screenComponent1 = new ScreenComponent()
            {
                ApplicationId = application.Id,
                Name = model.Name
            };
            await _context.AddAsync(screenComponent1).ConfigureAwait(false);
            metric = new ScreenComponentRender()
            {
                ComponentId = screenComponent1.Id,
                Date = DateTime.Now,
                RenderTime = model.RenderTime,
                DeviceInfoId = deviceInfo?.Id
            };
        }
        else
        {
            metric = new ScreenComponentRender()
            {
                ComponentId = screenComponent.Id,
                Date = DateTime.Now,
                RenderTime = model.RenderTime,
                DeviceInfoId = deviceInfo?.Id
            };
        }
        await _context.AddAsync(metric).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        return Ok(new ApiOkResponse(metric, "Metric successfully created"));
    }
    
    [HttpPost(Name = nameof(SubmitComponentCode))]
    public async Task<IActionResult> SubmitComponentCode([FromBody] SubmitComponentCodeViewModel model)
    {
        var application = await _context.Applications.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.UniqueAccessCode == model.UniqueAccessCode).ConfigureAwait(false);
    
        if (application == null)
        {
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"No Application with unique access code!"));
        }
    
        var screenComponent = await _context.ScreenComponents.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.Name == model.Name && d.ApplicationId == application.Id).ConfigureAwait(false);
    
        if (screenComponent == null)
        {
            var newComponent = new ScreenComponent()
            {
                ApplicationId = application.Id,
                Name = model.Name,
                SourceCode = model.SourceCode,
                Threshold = 100
            };
        
            await _context.AddAsync(newComponent).ConfigureAwait(false);
        }
        else
        {
            // Update existing component
            screenComponent.SourceCode = model.SourceCode;
            _context.Update(screenComponent);
        }
    
        await _context.SaveChangesAsync().ConfigureAwait(false);
    
        return Ok(new ApiOkResponse(null, "Component code submitted successfully"));
    }
    
    [HttpGet("{applicationId}",Name = nameof(GetComponents))]
    public async Task<IActionResult> GetComponents(string applicationId)
    {
        var items = await _context.ScreenComponents
            .Where(d => d.IsActive && !d.IsDeleted && d.ApplicationId == applicationId)
            .ToListAsync();

        return Ok(items.OrderByDescending(d => d.CreatedDate).ToList());
    }
    
[HttpGet("{componentId}", Name = nameof(GetComponent))]
public async Task<IActionResult> GetComponent(string componentId)
{
    var component = await _context.ScreenComponents
        .Include(c => c.ScreenComponentRenders)
            .ThenInclude(r => r.DeviceInfo)  // Include device info
        .Include(c => c.OptimizationSuggestions)
        .FirstOrDefaultAsync(c => c.Id == componentId);
    
    if (component == null)
    {
        return NotFound("Component not found");
    }

    var items = await _context.ScreenComponentRenders
        .Include(r => r.DeviceInfo)  // Include device info
        .Where(d => d.IsActive && !d.IsDeleted && d.ComponentId == componentId)
        .OrderByDescending(d => d.Date)
        .ToListAsync();
    
    List<ScreenComponentRender> recentRenders;
    if (items.Count < 100)
    {
        recentRenders = items; // Assign the entire list
    }
    else
    {
        recentRenders = items.GetRange(0, 100); // Get the first 100 items
    }
    DateTime today = DateTime.Today;

    // Daily average - renders from today
    var dailyRenders = items.Where(i => i.Date.Date == today).ToList();
    decimal dailyAvg = dailyRenders.Count > 0 
        ? dailyRenders.Sum(i => i.RenderTime) / dailyRenders.Count 
        : 0;
    
    // Weekly average - renders from the last 7 days
    var weeklyRenders = items.Where(i => i.Date >= today.AddDays(-7)).ToList();
    decimal weeklyAvg = weeklyRenders.Count > 0 
        ? weeklyRenders.Sum(i => i.RenderTime) / weeklyRenders.Count 
        : 0;

    // Monthly average - renders from the last 30 days
    var monthlyRenders = items.Where(i => i.Date >= today.AddDays(-30)).ToList();
    decimal monthlyAvg = monthlyRenders.Count > 0 
        ? monthlyRenders.Sum(i => i.RenderTime) / monthlyRenders.Count 
        : 0;

    // Overall average from recent renders
    decimal hundredAverage = recentRenders.Count > 0 
        ? recentRenders.Sum(i => i.RenderTime) / recentRenders.Count 
        : 0;
    
    decimal average = items.Count > 0 
        ? items.Sum(i => i.RenderTime) / items.Count 
        : 0;

    // Calculate device-specific statistics
    var deviceStats = items
        .Where(i => i.DeviceInfo != null)
        .GroupBy(i => new { 
            Brand = i.DeviceInfo.Brand ?? "unknown", 
            Model = i.DeviceInfo.ModelName ?? "unknown" 
        })
        .Select(g => new DeviceRenderStatsViewModel
        {
            DeviceBrand = g.Key.Brand,
            DeviceModel = g.Key.Model,
            RenderCount = g.Count(),
            AverageRenderTime = g.Average(i => (double)i.RenderTime)
        })
        .OrderBy(d => d.AverageRenderTime)
        .ToList();

    // Calculate OS-specific statistics
    var osStats = items
        .Where(i => i.DeviceInfo != null)
        .GroupBy(i => new { 
            OS = i.DeviceInfo.OsName ?? i.DeviceInfo.Os ?? "unknown", 
            Version = i.DeviceInfo.OsVersion ?? "unknown" 
        })
        .Select(g => new OsRenderStatsViewModel
        {
            OsName = g.Key.OS,
            OsVersion = g.Key.Version,
            RenderCount = g.Count(),
            AverageRenderTime = g.Average(i => (double)i.RenderTime)
        })
        .OrderBy(o => o.AverageRenderTime)
        .ToList();

    var stat = new RenderTimeStatisticsViewModel()
    {
        Name = component.Name,
        Last100Average = hundredAverage,
        DailyAverage = dailyAvg,
        WeeklyAverage = weeklyAvg,
        MonthlyAverage = monthlyAvg,
        Average = average,
        DeviceStats = deviceStats,  // Add device statistics
        OsStats = osStats          // Add OS statistics
    };

    // Get optimization suggestions
    var optimizationSuggestions = await _context.OptimizationSuggestions
        .Where(s => s.ComponentId == componentId && s.IsActive && !s.IsDeleted)
        .OrderByDescending(s => s.CreatedDate)
        .ToListAsync();

    var componentDetails = new ComponentViewModel()
    {
        Id = componentId,
        ApplicationId = component.ApplicationId,
        Name = component.Name,
        Statistics = stat,
        ScreenComponentRenders = items,
        OptimizationSuggestions = optimizationSuggestions,
        Threshold = component.Threshold
    };

    return Ok(componentDetails);
}
    
    [HttpGet("{componentId}",Name = nameof(GetRenderInstances))]
    public async Task<IActionResult> GetRenderInstances(string componentId)
    {
        var items = await _context.ScreenComponentRenders
            .Where(d => d.IsActive && !d.IsDeleted && d.ComponentId == componentId)
            .ProjectTo<RenderInstanceViewModel>(_configuration)
            .ToListAsync();

        return Ok(items.OrderByDescending(d => d.Date).ToList());
    }
    
    [HttpGet("{componentId}", Name = nameof(GetRenderTimeAverages))]
    public async Task<IActionResult> GetRenderTimeAverages(string componentId)
    {
        var component = await _context.ScreenComponents.FindAsync(componentId);
        if (component == null)
        {
            return NotFound("Component not found");
        }

        var metrics = await _context.ScreenComponentRenders
            .Include(r => r.DeviceInfo)  // Include device info
            .Where(m => m.ComponentId == componentId && m.IsActive && !m.IsDeleted)
            .OrderByDescending(m => m.Date)
            .ToListAsync();

        var startDate = DateTime.Today.AddDays(-14); // Last 15 days including today
        var averages = new List<DailyAverageViewModel>();

        // Group by device for more detailed analysis
        var metricsByDevice = metrics
            .Where(m => m.Date >= startDate)
            .GroupBy(m => m.DeviceInfo != null ? m.DeviceInfo.DeviceType : "unknown");

        // Process each day's data
        for (int i = 0; i <= 14; i++)
        {
            var date = startDate.AddDays(i);
            var dailyMetrics = metrics.Where(m => m.Date.Date == date.Date).ToList();

            var average = new DailyAverageViewModel
            {
                Date = date,
                Average = (decimal)(dailyMetrics.Any()
                    ? dailyMetrics.Average(d => (double)d.RenderTime)
                    : 0),
                Count = dailyMetrics.Count
            };

            // Add device breakdown if available
            if (dailyMetrics.Any(d => d.DeviceInfo != null))
            {
                average.DeviceBreakdown = dailyMetrics
                    .GroupBy(d => d.DeviceInfo?.DeviceType ?? "unknown")
                    .Select(g => new DeviceAverageViewModel
                    {
                        DeviceType = g.Key,
                        Average = g.Average(d => (double)d.RenderTime),
                        Count = g.Count()
                    })
                    .ToList();
            }

            averages.Add(average);
        }

        return Ok(averages);
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteComponentById))]
    public async Task<IActionResult> DeleteComponentById(string id)
    {
        var screenComponent = await _context.ScreenComponents.FirstOrDefaultAsync(n => n.Id == id);
        if (screenComponent == null)
        {
            return BadRequest("Component doesn't exist");
        }

        var items = await _context.ScreenComponentRenders.Where(i => i.ComponentId == id).ToListAsync();
        
        foreach (var i in items)
        {
            _context.ScreenComponentRenders.Remove(i);
        }
        _context.ScreenComponents.Remove(screenComponent);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new ApiOkResponse(screenComponent, $"Screen Component {screenComponent.Name} was deleted successfully"));
        }
        catch (DbUpdateConcurrencyException)
        {
            {
                throw;
            }
        }
    }
    
    [HttpPost(Name = nameof(MarkOptimizationImplemented))]
    public async Task<IActionResult> MarkOptimizationImplemented([FromBody] MarkOptimizationImplementedViewModel model)
    {
        var suggestion = await _context.OptimizationSuggestions
            .FirstOrDefaultAsync(s => s.Id == model.OptimizationSuggestionId)
            .ConfigureAwait(false);
    
        if (suggestion == null)
        {
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Optimization suggestion not found"));
        }
    
        suggestion.IsImplemented = true;
        suggestion.ImplementedDate = DateTime.Now;
    
        if (model.ResetMeasurements)
        {
            // Get the component
            var component = await _context.ScreenComponents
                .FirstOrDefaultAsync(c => c.Id == suggestion.ComponentId)
                .ConfigureAwait(false);
            
            if (component != null)
            {
                // Reset the threshold if needed
                component.Threshold = model.NewThreshold ?? component.Threshold;
                _context.Update(component);
            
                // Remove old render measurements if requested
                if (model.ClearOldMeasurements)
                {
                    var renders = await _context.ScreenComponentRenders
                        .Where(r => r.ComponentId == component.Id)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    foreach (var render in renders)
                    {
                        render.IsDeleted = true;
                        render.IsActive = false;
                    }

                    _context.UpdateRange(renders);
                }
            }
        }
    
        _context.Update(suggestion);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    
        return Ok(new ApiOkResponse(suggestion, "Optimization marked as implemented"));
    }
    
    [HttpGet("{id}", Name = nameof(GetOptimizationSuggestion))]
    public async Task<IActionResult> GetOptimizationSuggestion(string id)
    {
        var suggestion = await _context.OptimizationSuggestions
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if (suggestion == null)
        {
            return NotFound("Optimization suggestion not found");
        }
    
        return Ok(suggestion);
    }
    
    [HttpPost("", Name = nameof(UpdateComponent))]
    public async Task<IActionResult> UpdateComponent(UpdateScreenComponentViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Id) || string.IsNullOrWhiteSpace(vm.Name))
        {
            return BadRequest(new ApiResponse(400,$"Please enter correct component details"));
        }
        var component = await _context.ScreenComponents
            .FirstOrDefaultAsync(c => c.Id == vm.Id);
        
        if (component == null)
        {
            return NotFound("Component not found");
        }

        if (component.IsDeleted || !component.IsActive)
        {
            return BadRequest(new ApiResponse(400,$"Screen Component {component.Name} was deleted"));
        }

        component.Threshold = vm.Threshold;
        component.Name = vm.Name;
        _context.Update(component);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        if (vm.ClearMeasurements)
        {
            var renders = await _context.ScreenComponentRenders
                .Where(r => r.ComponentId == component.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var render in renders)
            {
                render.IsDeleted = true;
                render.IsActive = false;
            }

            _context.UpdateRange(renders);
        }
        
        return Ok(new ApiOkResponse(component, "Component update successfully"));
    }
    
}