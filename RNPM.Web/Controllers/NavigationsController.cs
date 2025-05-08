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
    public async Task<IActionResult> Index(string id)
    {
        if (id != null)
        {
            await SetData(id);
            return View();
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
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