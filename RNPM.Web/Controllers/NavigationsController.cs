using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RNPM.Common.ViewModels.Core;

namespace RNPM.Web.Controllers;
[Authorize]
public class NavigationsController : BaseController<NavigationsController>
{
    private readonly ILogger<NavigationsController> _logger;
    private readonly INotyfService _notyfService;

    public NavigationsController(ILogger<NavigationsController> logger, INotyfService notifyService)
    {
        _logger = logger;
        _notyfService = notifyService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(string applicationId)
    {
        if (applicationId != null)
        {
            var applications = await Get<List<ApplicationViewModel>>(await GetHttpClient(), "api/applications/getuserapplications",
                GetUserId());
            var navigations = await Get<List<NavigationViewModel>>(await GetHttpClient(), "api/navigations/getNavigations", applicationId);
            ViewBag.Applications = applications;
            ViewBag.Navigations = navigations;
            
            return View();
        }

        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        var applications = await Get<List<ApplicationViewModel>>(await GetHttpClient(), "api/applications/getuserapplications",
            GetUserId());
        // Get navigation details with device info
        var navigation = await Get<NavigationViewModel>(await GetHttpClient(), "api/navigations/getNavigation", id);
    
        // Get navigation averages with device breakdown
        var navigationAverages = await Get<List<DailyAverageViewModel>>(await GetHttpClient(), "api/navigations/getNavigationAverages", id);
        
        // Get device distribution for this navigation
        var deviceDistribution = await Get<DeviceDistributionViewModel>(await GetHttpClient(), "api/navigations/getNavigationDeviceDistribution", id);
    
        // Set up filter options
        var deviceTypes = navigation.NavigationInstances
            .Select(ni => ni.DeviceInfo?.DeviceType)
            .Where(dt => !string.IsNullOrEmpty(dt))
            .Distinct()
            .ToList();
    
        var osVersions = navigation.NavigationInstances
            .Select(ni => ni.DeviceInfo?.OsVersion)
            .Where(os => !string.IsNullOrEmpty(os))
            .Distinct()
            .ToList();
    
        ViewBag.Navigation = navigation;
        ViewBag.NavigationAverages = navigationAverages;
        ViewBag.DeviceDistribution = deviceDistribution;
        ViewBag.DeviceTypes = deviceTypes;
        ViewBag.OsVersions = osVersions;
        ViewBag.Applications = applications;
    
        return View();
    }
    
    private async Task SetData(string navigationId)
    {
        var applications = await Get<List<ApplicationViewModel>>(await GetHttpClient(), "api/applications/getuserapplications",
            GetUserId());
        var navigation = await Get<NavigationViewModel>(await GetHttpClient(), "api/Navigations/getNavigation", navigationId);
        var navigationAverages = await Get<List<DailyAverageViewModel>>(await GetHttpClient(), "api/Navigations/getNavigationAverages", navigationId);
        ViewBag.Applications = applications;
        ViewBag.NavigationAverages = navigationAverages;
        ViewBag.Navigation = navigation;
        
    }
        
    public async Task<ActionResult> DeleteNavigation(string id)
    {
        var navigationToDelete = await Get<NavigationViewModel>(await GetHttpClient(),
            "api/navigations/getNavigation", id);
            
        var component = await Remove<NavigationViewModel>(await GetHttpClient(), "api/navigations/deleteNavigationById", id);
            
        //return Json(value);
        return RedirectToAction("Dashboard","Home", new {id = navigationToDelete.ApplicationId});
    }
        
}