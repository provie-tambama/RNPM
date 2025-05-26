using System.Net;
using System.Net.Mime;
using RNPM.API.ViewModels.Core;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RNPM.CodeOptimizer.Requests;
using RNPM.CodeOptimizer.Services;
using RNPM.Common.Data;
using RNPM.Common.Data.Enums;
using RNPM.Common.Models;
using RNPM.Common.ViewModels.Core;
using Serilog;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace RNPM.Services.Core.Controllers;

[Route("api/[controller]/[action]")]
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class CodeOptimisationsController : Controller
{
    private readonly RnpmDbContext _context;
    private readonly ICodeOptimizerService _optimizerService;
    private readonly AutoMapper.IConfigurationProvider _configuration;

    public CodeOptimisationsController(RnpmDbContext context, IConfigurationProvider configuration, ICodeOptimizerService optimizerService)
    {
        _context = context;
        _configuration = configuration;
        _optimizerService = optimizerService;
    }
    
    [HttpPost(Name = nameof(RequestOptimisationSuggestion))]
    public async Task<IActionResult> RequestOptimisationSuggestion([FromBody] string code)
    {
        var response = await _optimizerService.OptimizeCodeSubAsync(new CodeOptimizationRequest
        {
            ComponentId = "test",
            ComponentName = "My Component",
            Code = code,
            ComponentType = "Class"
        });
        
        return Ok(new ApiOkResponse(response, "Metric successfully created"));
    }
    
}