using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RNPM.Common.ViewModels.Core;

namespace RNPM.Web.Controllers;
[Authorize]
public class RequestsController : BaseController<RequestsController>
{
    private readonly ILogger<RequestsController> _logger;
    private readonly INotyfService _notyfService;

    public RequestsController(ILogger<RequestsController> logger, INotyfService notifyService)
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
    
    private async Task SetData(string requestId)
    {
        var applications = await Get<List<ApplicationViewModel>>(await GetHttpClient(), "api/applications/getuserapplications",
            GetUserId());
        var request = await Get<NetworkRequestViewModel>(await GetHttpClient(), "api/NetworkRequests/getNetworkRequest", requestId);
        var requestAverages = await Get<List<DailyAverageViewModel>>(await GetHttpClient(), "api/NetworkRequests/getNetworkRequestAverages", requestId);
        ViewBag.Applications = applications;
        ViewBag.Request = request;
        ViewBag.RequestAverages = requestAverages;
    }
        
    public async Task<ActionResult> DeleteRequest(string id)
    {
        var requestToDelete = await Get<NetworkRequestViewModel>(await GetHttpClient(),
            "api/navigations/getNavigation", id);
            
        var request = await Remove<NetworkRequestViewModel>(await GetHttpClient(), "api/networkRequests/deleteNetworkRequestById", id);
            
        //return Json(value);
        return RedirectToAction("Dashboard","Home", new {id = requestToDelete.ApplicationId});
    }
        
}