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
        try
        {
            var categories = await context.Categories.ToListAsync();
            return Ok(categories);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("/v1/categories/{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPost("/v1/categories")]
    public async Task<IActionResult> PostAsync([FromBody] Category category,
        [FromServices] BlogDataContext context)
    {
        try
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return Created($"v1/categories/{category.Id}", category);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPut("/v1/categories/{id:int}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id,
        [FromBody] Category model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            category.Name = model.Name;
            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(category);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpDelete("/v1/categories/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(category);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}