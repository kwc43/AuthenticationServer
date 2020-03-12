using AuthenticationServer.Core.Entities;
using AuthenticationServer.Infrastructure.Data;
using AuthenticationServer.Models.Login;
using AuthenticationServer.Models.Register;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthenticationServer.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppIdentityDbContext _appIdentityDbContext;
        private readonly IEventService _eventService;

        public AccountsController(
            SignInManager<AppUser> signInManager,
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            UserManager<AppUser> userManager,
            AppIdentityDbContext appIdentityDbContext,
            IEventService eventService
        )
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _userManager = userManager;
            _appIdentityDbContext = appIdentityDbContext;
            _eventService = eventService;
        }

        [Route("api/[controller]")]
        public async Task<IActionResult> Post([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.Email, FullName = model.FullName, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.FullName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", model.Role));

            return Ok(new RegisterResponse(user, model.Role));
        }

        [HttpGet]
        public  async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            return View(new LoginViewModel
                {
                    ReturnUrl = returnUrl,
                    Username = GetUserName(returnUrl) ?? context?.LoginHint,
                    NewAccount = returnUrl.Contains("newAccount")
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Username);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.FullName));

                    await HttpContext.SignInAsync(user.Id, user.UserName);

                    if (context != null)
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Url.IsLocalUrl(model.ReturnUrl) ? Redirect(model.ReturnUrl) :
                        string.IsNullOrEmpty(model.ReturnUrl) ? Redirect("~/") :
                        throw new Exception("invalid return URL");
                }

                await _eventService.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
            }

            var vm = new LoginViewModel
            {
                Username = model.Username,
                RememberLogin = model.RememberLogin
            };

            return View(vm);
        }

        private static string GetUserName(string returnUrl)
        {
            const string parameter = "&userName=";
            return returnUrl.Contains("userName") ? returnUrl.Substring(returnUrl.IndexOf("&userName=") + parameter.Length) : null;
        }
    }
}
