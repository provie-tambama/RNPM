using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RNPM.Common.Data;
using RNPM.Common.Data.Enums;
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
public class NetworkRequestsController : Controller
{
    private readonly RnpmDbContext _context;
    private readonly AutoMapper.IConfigurationProvider _configuration;

    public NetworkRequestsController(RnpmDbContext context, IConfigurationProvider configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [HttpPost(Name = nameof(CreateNetworkRequestMetric))]
    public async Task<IActionResult> CreateNetworkRequestMetric([FromBody] CreateNetworkRequestViewModel model)
    {
        var application = await _context.Applications.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.UniqueAccessCode == model.UniqueAccessCode).ConfigureAwait(false);
        if (application == null)
        {
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, $"No Application with unique access code!"));
        }
        var request = await _context.NetworkRequests.FirstOrDefaultAsync(d =>
            d.IsActive && !d.IsDeleted && d.Name == model.Name && d.ApplicationId == application.Id).ConfigureAwait(false);
        HttpRequestInstance metric;
        if (request == null)
        {
           var request1 = new NetworkRequest()
            {
                ApplicationId = application.Id,
                Name = model.Name
            };
            await _context.AddAsync(request1).ConfigureAwait(false);
            metric = new HttpRequestInstance()
            {
                RequestId = request1.Id,
                RequestDate = DateTime.Now,
                RequestCompletionTime = model.RequestCompletionTime
            };
        }
        else
        {
            metric = new HttpRequestInstance()
            {
                RequestId = request.Id,
                RequestDate = DateTime.Now,
                RequestCompletionTime = model.RequestCompletionTime
            };
        }
        await _context.AddAsync(metric).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        
        return Ok(new ApiOkResponse(metric, "Metric successfully created"));
    }
    
    [HttpGet("{applicationId}",Name = nameof(GetNetworkRequests))]
    public async Task<IActionResult> GetNetworkRequests(string applicationId)
    {
        var items = await _context.NetworkRequests
            .Where(d => d.IsActive && !d.IsDeleted && d.ApplicationId == applicationId)
            .ToListAsync();

        return Ok(items.OrderByDescending(d => d.CreatedDate).ToList());
    }

    [HttpGet("{requestId}", Name = nameof(GetNetworkRequest))]
    public async Task<IActionResult> GetNetworkRequest(string requestId)
    {
        var request = await _context.NetworkRequests.Include(c => c.HttpRequestInstances).FirstOrDefaultAsync(c => c.Id == requestId);
        var items = await _context.HttpRequestInstances
            .Where(d => d.IsActive && !d.IsDeleted && d.RequestId == requestId).OrderByDescending(d =>d.RequestDate)
            .ToListAsync();
        List<HttpRequestInstance> recentRequestInstances;
        if (items.Count < 100)
        {
            recentRequestInstances = items;
        }
        else
        {
           recentRequestInstances = items.GetRange(0, 100);
        }
        var sum = (recentRequestInstances.Sum(i => i.RequestCompletionTime));
        var average = sum / recentRequestInstances.Count;
        string insight;
        MetricStatus status;
        string comment;
    if (average < 200)
    {
        status = MetricStatus.Excellent;
        insight = "Network request time is excellent.";
        comment = "Your network request performance is excellent, ensuring a smooth user experience. " +
                  "To maintain this performance, continue optimizing your server-side code, " +
                  "minimize payload sizes, and leverage caching where appropriate.";
    }
    else if (average < 500)
    {
        status = MetricStatus.Good;
        insight = "Network request time is good.";
        comment = "Your network request performance is good. To achieve excellence, consider optimizing " +
                  "server response times, reducing payload sizes, and utilizing content delivery networks (CDNs).";
    }
    else if (average < 1000)
    {
        status = MetricStatus.Acceptable;
        insight = "Network request time is acceptable.";
        comment = "Your network request performance is acceptable but can be improved. Check for opportunities " +
                  "to optimize API endpoints, compress data, and reduce the frequency of network calls.";
    }
    else if (average < 2000)
    {
        status = MetricStatus.NeedsImprovement;
        insight = "Network request time needs improvement.";
        comment = "Your network request performance needs improvement. Investigate slow responses, " +
                  "optimize backend processes, and ensure efficient data handling. Consider using asynchronous " +
                  "requests to improve perceived performance.";
    }
    else if (average < 3000)
    {
        status = MetricStatus.Poor;
        insight = "Network request time is poor.";
        comment = "Your network request performance is poor and negatively affects user experience. " +
                  "Profile your API endpoints, optimize database queries, and ensure your server infrastructure " +
                  "is adequately scaled. Implement caching strategies and reduce the amount of data transferred.";
    }
    else
    {
        status = MetricStatus.VeryPoor;
        insight = "Network request time is very poor.";
        comment = "Your network request performance is very poor, causing significant delays and frustration for users. " +
                  "Consider this a critical issue. Profile your entire network stack to identify bottlenecks, " +
                  "optimize your server and database performance, and implement aggressive caching and data compression strategies.";
    }
        var stat = new NetworkRequestStatisticsViewModel()
        {
            Average = average,
            Insight = insight,
            Status = status,
            Comment = comment
        };

        var requestDetails = new NetworkRequestViewModel()
        {
            Id = requestId,
            ApplicationId = request?.ApplicationId,
            Name = request?.Name,
            Statistics = stat,
            HttpRequestInstances = recentRequestInstances
        };
        return Ok(requestDetails);
    }
    
    [HttpGet("{requestId}", Name = nameof(GetNetworkRequestAverages))]
    public async Task<IActionResult> GetNetworkRequestAverages(string requestId)
    {
        var metrics = await _context.HttpRequestInstances
            .Where(m => m.RequestId == requestId)
            .ToListAsync();

        var dailyAverages = metrics
            .GroupBy(m => m.RequestDate.Date)
            .OrderByDescending(g => g.Key)
            .Take(15)
            .Select(g => new DailyAverageViewModel
            {
                Date = g.Key,
                Average = g.Average(m => m.RequestCompletionTime)
            })
            .ToList();

        return Ok(dailyAverages);
    }
    
    [HttpDelete("{id}", Name = nameof(DeleteNetworkRequestById))]
    public async Task<IActionResult> DeleteNetworkRequestById(string id)
    {
        var request = await _context.NetworkRequests.FirstOrDefaultAsync(n => n.Id == id);
        if (request == null)
        {
            return BadRequest("Request doesn't exist");
        }

        var items = await _context.HttpRequestInstances.Where(i => i.RequestId == id).ToListAsync();
        
        foreach (var i in items)
        {
            _context.HttpRequestInstances.Remove(i);
        }

        _context.NetworkRequests.Remove(request);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new ApiOkResponse(request, $"Request {request.Name} was deleted successfully"));
        }
        catch (DbUpdateConcurrencyException)
        {
            {
                throw;
            }
        }
    }
}