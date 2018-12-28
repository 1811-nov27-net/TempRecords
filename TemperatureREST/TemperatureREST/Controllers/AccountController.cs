using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TemperatureREST.Models;

namespace TemperatureREST.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private SignInManager<IdentityUser> _signInManager { get; }

        // the AddIdentity in Startup.cs also registered a bunch of services
        // including SignInManager, RoleManager, etc
        public AccountController(SignInManager<IdentityUser> signInManager, IdentityDbContext db)
        {
            db.Database.EnsureCreated();
            _signInManager = signInManager;
        }

        // we're gonna handwave over REST uniform interface for authentication type stuff
        [HttpPost]
        public async Task<ActionResult> Login(AccountViewModel account)
        {
            // access the identity DB and attempt login with password
            var result = await _signInManager.PasswordSignInAsync(account.UserName, account.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return StatusCode(403); // Forbidden, invalid login
            }

            return NoContent();
        }

        [HttpPost]
        // account from body, admin bool from query string
        public async Task<ActionResult> Register(AccountViewModel account,
            [FromServices] UserManager<IdentityUser> userManager,
            [FromServices] RoleManager<IdentityRole> roleManager, bool admin = false)
        {
            var user = new IdentityUser(account.UserName);

            var result = await userManager.CreateAsync(user, account.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result); // probably duplicate username or invalid characters
            }

            // configure roles
            if (admin)
            {
                var result2 = await roleManager.RoleExistsAsync("admin");
                if (!result2)
                {
                    // we need to create the admin role first
                    var adminRole = new IdentityRole("admin");
                    result = await roleManager.CreateAsync(adminRole);
                    if (!result.Succeeded)
                    {
                        return StatusCode(500, result);
                    }
                }
                // now we know admin role exists
                result = await userManager.AddToRoleAsync(user, "admin");
                if (!result.Succeeded)
                {
                    return StatusCode(500, result);
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return NoContent();
        }

        public async Task<NoContentResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public string LoggedInUser()
        {
            // we can access User object from any Controller, it has logged-in user details
            var roles = User.IsInRole("admin");
            return User.Identity.Name;
        }
    }
}