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
                NavigationCompletionTime = model.NavigationCompletionTime
            };
        }
        else
        {
            metric = new NavigationInstance()
            {
                NavigationId = navigation.Id,
                Date = DateTime.Now,
                NavigationCompletionTime = model.NavigationCompletionTime
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
        var navigation = await _context.Navigations.Include(c => c.NavigationInstances).FirstOrDefaultAsync(c => c.Id == navigationId);
        var items = await _context.NavigationInstances
            .Where(d => d.IsActive && !d.IsDeleted && d.NavigationId == navigationId).OrderByDescending(d =>d.Date)
            .ToListAsync();
        List<NavigationInstance> recentNavigationInstances;
        if (items.Count < 100)
        {
            recentNavigationInstances = items; // Assign the entire list
        }
        else
        {
           recentNavigationInstances = items.GetRange(0, 100); // Get the first 15 items
        }
        var sum = (recentNavigationInstances.Sum(i => i.NavigationCompletionTime));
        var average = sum / recentNavigationInstances.Count;
        string insight;
        MetricStatus status;
        string comment;
    if (average < 100)
    {
        status = MetricStatus.Excellent;
        insight = "Navigation time is excellent.";
        comment = "Your navigation performance is excellent, ensuring a seamless user experience. " +
                  "Maintain this performance by keeping navigation logic simple and avoiding heavy computations during navigation.";
    }
    else if (average < 300)
    {
        status = MetricStatus.Good;
        insight = "Navigation time is good.";
        comment = "Your navigation performance is good. To achieve excellence, consider optimizing any data fetching " +
                  "operations performed during navigation and ensure minimal component re-renders.";
    }
    else if (average < 500)
    {
        status = MetricStatus.Acceptable;
        insight = "Navigation time is acceptable.";
        comment = "Your navigation performance is acceptable but can be improved. Check for any unnecessary " +
                  "operations performed during navigation and optimize them. Consider lazy loading for non-critical components.";
    }
    else if (average < 1000)
    {
        status = MetricStatus.NeedsImprovement;
        insight = "Navigation time needs improvement.";
        comment = "Your navigation performance needs improvement. Investigate the causes of slow navigation, " +
                  "such as data fetching, synchronous operations, or complex component hierarchies. Optimize your navigation logic accordingly.";
    }
    else if (average < 2000)
    {
        status = MetricStatus.Poor;
        insight = "Navigation time is poor.";
        comment = "Your navigation performance is poor and noticeably affects user experience. Profile your navigation process " +
                  "to identify bottlenecks and optimize data fetching and component rendering during navigation.";
    }
    else
    {
        status = MetricStatus.VeryPoor;
        insight = "Navigation time is very poor.";
        comment = "Your navigation performance is very poor, leading to significant delays and a frustrating user experience. " +
                  "Consider this a critical issue. Profile your application to identify the causes of the delay and implement " +
                  "asynchronous data loading, optimize component rendering, and simplify navigation logic.";
    }
        var stat = new NavigationStatisticsViewModel()
        {
            Average = average,
            Insight = insight,
            Status = status,
            Comment = comment
        };

        var navigationDetails = new NavigationViewModel()
        {
            Id = navigationId,
            ApplicationId = navigation?.ApplicationId,
            FromScreen = navigation?.FromScreen,
            ToScreen = navigation?.ToScreen,
            Statistics = stat,
            NavigationInstances = recentNavigationInstances
        };
        return Ok(navigationDetails);
    }

    [HttpGet("{navigationId}", Name = nameof(GetNavigationAverages))]
    public async Task<IActionResult> GetNavigationAverages(string navigationId)
    {
        var metrics = await _context.NavigationInstances
            .Where(m => m.NavigationId == navigationId)
            .ToListAsync();

        var dailyAverages = metrics
            .GroupBy(m => m.Date.Date)
            .OrderByDescending(g => g.Key)
            .Take(15)
            .Select(g => new DailyAverageViewModel
            {
                Date = g.Key,
                Average = g.Average(m => m.NavigationCompletionTime)
            })
            .ToList();

        return Ok(dailyAverages);
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