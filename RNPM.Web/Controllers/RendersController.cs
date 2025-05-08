using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RNPM.Common.ViewModels.Core;

namespace RNPM.Web.Controllers;
[Authorize]
public class RendersController : BaseController<RendersController>
{
    private readonly ILogger<RendersController> _logger;
    private readonly INotyfService _notyfService;

    public RendersController(ILogger<RendersController> logger, INotyfService notifyService)
    {
        _logger = logger;
        _notyfService = notifyService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(string id)
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
    
        private async Task SetData(string componentId)
        {
           var applications = await Get<List<ApplicationViewModel>>(await GetHttpClient(), "api/applications/getuserapplications",
                    GetUserId());
           var component = await Get<ComponentViewModel>(await GetHttpClient(), "api/screenComponents/getComponent", componentId);
           var renderAverages = await Get<List<DailyAverageViewModel>>(await GetHttpClient(), "api/screenComponents/getRenderTimeAverages", componentId);
            ViewBag.Applications = applications;
            ViewBag.ScreenComponent = component;
            ViewBag.RenderAverages = renderAverages;
        }
        
        public async Task<ActionResult> DeleteComponent(string id)
        {
            var componentToDelete = await Get<ScreenComponentViewModel>(await GetHttpClient(),
                "api/screenComponents/getComponent", id);
            
            var component = await Remove<ScreenComponentViewModel>(await GetHttpClient(), "api/screenComponents/deleteComponentById", id);
            
            //return Json(value);
            return RedirectToAction("Dashboard","Home", new {id = componentToDelete.ApplicationId});
        }
        
}