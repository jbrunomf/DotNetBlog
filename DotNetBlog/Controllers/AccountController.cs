using DotNetBlog.Services;
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

    [Authorize(Roles = "user")]
    [HttpGet("user")]
    public IActionResult Getuser()
    {
        return Ok(User.Identity.Name);
    }

    [Authorize(Roles = "author")]
    [HttpGet("author")]
    public IActionResult GetAuthor()
    {
        return Ok(User.Identity.Name);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("admin")]
    public IActionResult GetAdmin()
    {
        return Ok(User.Identity.Name);
    }
}