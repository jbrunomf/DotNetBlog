using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetBlog.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> Index([FromServices]BlogDataContext context)
    {
        var categories = await context.Categories.ToListAsync();
        return Ok(categories);
    }
}