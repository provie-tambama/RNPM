using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Dohwe.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RNPM.API.ViewModels;
using RNPM.Common.Constants;
using RNPM.Common.ViewModels;
using RNPM.Common.ViewModels.Core;
using RNPM.Common.ViewModels.UserManagement;
using RNPM.Web.Models;
using LoginViewModel = RNPM.Common.ViewModels.LoginViewModel;

namespace RNPM.Web.Controllers;
[Authorize]
public class HomeController : BaseController<HomeController>
{
    private readonly ILogger<HomeController> _logger;
    private readonly INotyfService _notyfService;
    public HomeController(ILogger<HomeController> logger, INotyfService notifyService)
    {
        _logger = logger;
        _notyfService = notifyService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await SetData();

        return View();
    }
    
        private async Task SetData()
        {
           var applications = await Get<List<ApplicationViewModel>>(await GetHttpClient(), "api/applications/getuserapplications",
                    GetUserId());
                ViewBag.Applications = applications;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateApplication([Bind($"Name")] string name)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                _notyfService.Error("An error occured");
                return RedirectToAction("Index");
            }
            var model = new CreateApplicationViewModel()
            {
                Name = name,
                UserId = GetUserId()
            };
            if (model.Name != "" || model.UserId != "")
            {
                var applicationCreation = await Add<ApplicationViewModel, CreateApplicationViewModel>(await GetHttpClient(), "api/applications/createapplication", model);
                if ( applicationCreation.Status != (int)HttpStatusCode.OK)
                {
                    ModelState.AddModelError("", applicationCreation.Title);
                    _notyfService.Error(applicationCreation.Title);

                    return RedirectToAction("Index");
                }
                var application = applicationCreation.Result;
                _notyfService.Success(applicationCreation.Title);
                await SetData();
                return RedirectToAction("Index");
            }

            await SetData();
            ViewBag.Message = "applicationCreation.Title";

            return RedirectToAction("Index");
        }
        public async Task<ActionResult> DeleteApplication(string id)
        {
            var applicationToDelete = await Get<ApplicationViewModel>(await GetHttpClient(),
                "api/applications/getApplication", id);
            
            var component = await Remove<ApplicationViewModel>(await GetHttpClient(), "api/applications/deleteApplicationById", id);
            if (component.Status == (int)HttpStatusCode.OK)
            {
                _notyfService.Error("Failed to delete! Contact system administrator");
            }
            else
            {
                _notyfService.Success(component.Title);
            }
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Dashboard(string id)
        {
            var application = await Get<ApplicationViewModel>(await GetHttpClient(), "api/applications/GetApplication", id);
            var applicationId = application.Id;
            await SetData();
            ViewData["Application"] = application;
            ViewBag.Application = application;
            var components = await Get<List<ScreenComponentViewModel>>(await GetHttpClient(),
                "api/screenComponents/getComponents", applicationId);
            var navigations = await Get<List<NavigationViewModel>>(await GetHttpClient(),
                "api/navigations/getNavigations", applicationId);
            var networkRequests = await Get<List<NetworkRequestViewModel>>(await GetHttpClient(),
                "api/networkRequests/getNetworkRequests", applicationId);
            ViewBag.ScreenComponents = components;
            ViewBag.Navigations = navigations;
            ViewBag.NetworkRequests = networkRequests;
            return View(application);
        }
        

        
}