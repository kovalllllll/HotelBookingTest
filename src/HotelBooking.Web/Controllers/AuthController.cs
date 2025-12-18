using HotelBooking.Application.Models.Auth;
using HotelBooking.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole> roleManager)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResultModel>> Register([FromBody] RegisterModel model)
    {
        if (model.Password != model.ConfirmPassword)
        {
            return BadRequest(new AuthResultModel { Succeeded = false, Error = "Passwords do not match" });
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new AuthResultModel
            {
                Succeeded = false,
                Error = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        if (!await roleManager.RoleExistsAsync("Client"))
        {
            await roleManager.CreateAsync(new IdentityRole("Client"));
        }

        await userManager.AddToRoleAsync(user, "Client");

        await signInManager.SignInAsync(user, isPersistent: false);

        var roles = await userManager.GetRolesAsync(user);

        return Ok(new AuthResultModel
        {
            Succeeded = true,
            User = new UserModel
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            }
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultModel>> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest(new AuthResultModel { Succeeded = false, Error = "Invalid email or password" });
        }

        var result =
            await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return BadRequest(new AuthResultModel { Succeeded = false, Error = "Invalid email or password" });
        }

        var roles = await userManager.GetRolesAsync(user);

        return Ok(new AuthResultModel
        {
            Succeeded = true,
            User = new UserModel
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            }
        });
    }

    [HttpDelete("session")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpGet("current")]
    public async Task<ActionResult<UserModel>> GetCurrentUser()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }

        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);

        return Ok(new UserModel
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Roles = roles
        });
    }
}