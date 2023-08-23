using Microsoft.AspNetCore.Mvc;


namespace DotNetBlog.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet("health-check")]
    public IActionResult Get()
    {
        return Ok();
    }
}