using Blog.Data;
using Blog.Models;
using DotNetBlog.ExtensionMethods;
using DotNetBlog.Services;
using DotNetBlog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetBlog.Controllers;

[ApiController]
[Route("v1")]
public class AccountController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromServices]TokenService tokenService)
    {
        var token = tokenService.GenerateToken(null);
        return Ok(token);
    }

    [HttpPost("v1/accounts")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        }
        
    //     var user = new User
    //     {
    //         Name = model.Name,
    //         Email = model.Email,
    //         Roles =
    //     }
    return Ok();
    }
}