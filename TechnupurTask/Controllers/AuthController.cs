using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechnupurTask.Core.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechnupurTask.Helper;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly TokenHelper tokenHelper;
    public AuthController(TokenHelper tokenHelper, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {   
        this.tokenHelper = tokenHelper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var token = tokenHelper.GenerateToken(model.Username);

                // Implement logic to generate and return a token or simply return a success message
                return Ok(token);
            }
            else if (result.IsLockedOut)
            {
                return BadRequest("User account locked.");
            }
        }

        return BadRequest("Invalid login attempt.");
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    public async Task<IActionResult> Register(UserRegistrationModel model)
    {
        var user = new IdentityUser { UserName = model.Username, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");  // Default role

            // Handle post-registration logic
            return Ok("User registered.");
        }
        return BadRequest(result.Errors);
    }


    
}
