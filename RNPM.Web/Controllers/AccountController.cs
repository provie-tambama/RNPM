using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Dohwe.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RNPM.API.ViewModels;
using RNPM.Common.Constants;
using RNPM.Common.ViewModels;
using RNPM.Common.ViewModels.UserManagement;
using RNPM.Web.Models;
using LoginViewModel = RNPM.Common.ViewModels.LoginViewModel;

namespace RNPM.Web.Controllers;
public class AccountController : BaseController<AccountController>
{
    private readonly ILogger<AccountController> _logger;
    private readonly INotyfService _notyfService;

    public AccountController(ILogger<AccountController> logger, INotyfService notifyService)
    {
        _logger = logger;
        _notyfService = notifyService;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string message)
    {
        ViewBag.Message = message;
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login([Bind("Username,Password")] LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var login = (await Add<UserViewModel, LoginViewModel>(await GetHttpClient(), "api/auth/login", model));
            
            if (login == null)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong, try again");
                return View(model);
            }
            
            if ( login.Status != (int)HttpStatusCode.OK )
            {
                ModelState.AddModelError("", login.Title);

                return View(model);
            }

            var user = login.Result;

            var handler = new JwtSecurityTokenHandler();

            if (handler.ReadToken(user.Token) is JwtSecurityToken jwtToken)
            {
                var claims = jwtToken.Claims.ToList();

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var userPrincipal = new ClaimsPrincipal(new[] { claimsIdentity });
                await HttpContext.SignInAsync(userPrincipal);
            }

            HttpContext.Session.SetString("access_token", user.Token);
            HttpContext.Session.SetObject(SessionConstants.User, user);
            
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Something went wrong, try again");
        return View(model);
        
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string message)
    {
        ViewBag.Message = message;
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await Add<UserViewModel, RegisterUserViewModel>(await GetHttpClient(), "api/auth/register", model);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong, try again");
                return View(model);
            }
            if (user.Status == (int)HttpStatusCode.OK)
            {
                ViewBag.Message = user.Title;

                return View(nameof(Login));
            }

            ModelState.AddModelError(string.Empty, user.Title);
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Something went wrong, try again");
        return View(model);
    }
    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login", "Account");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}