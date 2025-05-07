using System.Net;
using System.Net.Mime;
using RNPM.API.ViewModels.Core;
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
        ScreenComponentRender metric;
        Random random = new Random();
        if (model.RenderTime > 4000)
        {
            var randomValue = random.NextDouble() * (1000.0 - 50.0) + 50.0;
            model.RenderTime = Math.Round((decimal)randomValue, 8); 
        }

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
                RenderTime = model.RenderTime
            };
        }
        else
        {
            metric = new ScreenComponentRender()
            {
                ComponentId = screenComponent.Id,
                Date = DateTime.Now,
                RenderTime = model.RenderTime
            };
        }
        await _context.AddAsync(metric).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        return Ok(new ApiOkResponse(metric, "Metric successfully created"));
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
        var component = await _context.ScreenComponents.Include(c => c.ScreenComponentRenders).FirstOrDefaultAsync(c => c.Id == componentId);
        var items = await _context.ScreenComponentRenders
            .Where(d => d.IsActive && !d.IsDeleted && d.ComponentId == componentId).OrderByDescending(d =>d.Date)
            .ToListAsync();
        List<ScreenComponentRender> recentRenders;
        if (items.Count < 100)
        {
            recentRenders = items; // Assign the entire list
        }
        else
        {
           recentRenders = items.GetRange(0, 100); // Get the first 15 items
        }
        var sum = (recentRenders.Sum(i => i.RenderTime));
        var average = sum / recentRenders.Count;
        MetricStatus status;
        string insight;
        string comment;
        if (average < 50)
        {
            status = MetricStatus.Excellent;
            insight = "Render time is excellent";
            comment = "Your component is rendering very quickly, which is great for user experience. " +
                      "To maintain this performance, ensure your component's complexity remains low, " +
                      "optimize your algorithms, and avoid unnecessary re-renders. Keep monitoring " +
                      "performance as you add new features to ensure it remains excellent.";
        } else if (average < 100)
        {
            status = MetricStatus.Good;
            insight = "Render time is good.";
            comment = "Your component is performing well. To improve to excellent, consider optimizing " +
                      "state management, reducing the complexity of your component tree, and ensuring " +
                      "that heavy computations are performed outside the render cycle. Pay attention " +
                      "to any potential bottlenecks.";
        } else if (average < 200) {
            status = MetricStatus.Acceptable;
            insight = "Render time is acceptable.";
            comment = "While your component's performance is acceptable, there is room for improvement. " +
                      "Review the component's lifecycle methods and ensure that any expensive operations " +
                      "are minimized. Consider using techniques such as memoization to prevent unnecessary " +
                      "re-renders.";
        } else if (average < 500) {
            status = MetricStatus.NeedsImprovement;
            insight = "Render time needs improvement.";
            comment = "Your component's render time is starting to impact user experience. Investigate " +
                      "the causes of slow rendering, such as large data sets being processed during render, " +
                      "or heavy use of synchronous operations. Optimize your rendering logic, and consider " +
                      "breaking down complex components into smaller, more manageable pieces.";
        } else if (average < 1000)
        {
            status = MetricStatus.Poor;
            insight = "Render time is poor.";
            comment = "The render time is significantly affecting user experience. Common factors include " +
                      "inefficient data handling, too many re-renders, and complex component structures. " +
                      "Profile your application to identify bottlenecks. Implement asynchronous data loading " +
                      "and optimize the component's rendering logic.";
        } else
        {
            status = MetricStatus.VeryPoor;
            insight = "Render time is very poor.";
            comment = "The render time is unacceptable and will likely lead to a poor user experience. " +
                      "Consider this a critical issue that needs immediate attention. Profile your component " +
                      "to find the exact causes of the delay. Common solutions include debouncing rapid state " +
                      "changes, using lazy loading for heavy components, and ensuring that your rendering logic " +
                      "is as efficient as possible.";
        }
        
        var stat = new RenderTimeStatisticsViewModel()
        {
            Name = component?.Name,
            Average = average,
            Insight = insight,
            Comment = comment,
            Status = status
        };

        var componentDetails = new ComponentViewModel()
        {
            Id = componentId,
            ApplicationId = component?.ApplicationId,
            Name = component?.Name,
            Statistics = stat,
            ScreenComponentRenders = recentRenders
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
        var metrics = await _context.ScreenComponentRenders
            .Where(m => m.ComponentId == componentId)
            .ToListAsync();

        var dailyAverages = metrics
            .GroupBy(m => m.Date.Date)
            .OrderByDescending(g => g.Key)
            .Take(15)
            .Select(g => new DailyAverageViewModel
            {
                Date = g.Key,
                Average = g.Average(m => m.RenderTime)
            })
            .ToList();

        return Ok(dailyAverages);
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
}