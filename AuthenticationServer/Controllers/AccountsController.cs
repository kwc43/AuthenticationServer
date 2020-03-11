using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Core.Entities;
using AuthenticationServer.Infrastructure.Data;
using AuthenticationServer.Models.Register;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public  IActionResult Login()
        {
            return View();
        }
    }
}
