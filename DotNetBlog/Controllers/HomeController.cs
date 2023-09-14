using DotNetBlog.Attributes;
using Microsoft.AspNetCore.Mvc;


namespace DotNetBlog.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet("health-check")]
    [ApiKey]
    public IActionResult Get()
    {
        return Ok();
    }
}