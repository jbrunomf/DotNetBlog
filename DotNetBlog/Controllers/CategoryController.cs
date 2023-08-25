using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DotNetBlog.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    [HttpGet("/v1/categories")]
    public async Task<IActionResult> Index([FromServices]BlogDataContext context)
    {
        var categories = await context.Categories.ToListAsync();
        return Ok(categories);
    }

    [HttpGet("/v1/categories/{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost("/v1/categories")]
    public async Task<IActionResult> PostAsync([FromBody] Category category,
        [FromServices] BlogDataContext context)
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        return Created($"v1/categories/{category.Id}", category);
    }
}