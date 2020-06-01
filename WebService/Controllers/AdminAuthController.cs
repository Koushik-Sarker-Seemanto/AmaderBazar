using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
using Newtonsoft.Json;
using Services.Contracts;

namespace WebService.Controllers
{
    public class AdminAuthController: Controller
    {
        private readonly IUserServices _userServices;
        private readonly ILogger<AdminAuthController> _logger;
        private string _authorSecret;
        
        public AdminAuthController(IUserServices userServices, ILogger<AdminAuthController> logger)
        {
            _userServices = userServices;
            this._logger = logger;

            var config = new ConfigurationBuilder();
            var configuration = config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            
            _authorSecret = configuration["AuthorSecret"];
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "AdminAuth");
        }

        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind] RegisterViewModel model)
        {
            if(ModelState.IsValid == false)
            {
                return View(model);
            }

            if (model.AuthorSecret != _authorSecret)
            {
                ModelState.AddModelError("", "Invalid Author Secret");
                return View();
            }

            var result = await _userServices.RegisterUser(model);
            if (result!=null)
            {
                return RedirectToAction("Login", "AdminAuth");
            }
            
            ModelState.AddModelError("", "Invalid Registration");
            return View(model);
        }
        
        public async Task<IActionResult> Login()
        {
            var authenticationInfo = await HttpContext.AuthenticateAsync();

            if(authenticationInfo != null && authenticationInfo.Succeeded)
            {
                return RedirectToAction("Index", "AdminPanel");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] LoginViewModel model)
        {
            if(ModelState.IsValid == false)
            {
                return View(model);
            }
            var user = await _userServices.Authenticate(model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Username or password is incorrect.");
                return View(model);
            }
            _logger.LogInformation($"User: {JsonConvert.SerializeObject(user)}");
            var claimsIdentity = _userServices.GetSecurityClaims(user, CookieAuthenticationDefaults.AuthenticationScheme);

            var authenticationProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties
            );
            return RedirectToAction("Index", "AdminPanel");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login","AdminAuth");
        }
        
    }
}