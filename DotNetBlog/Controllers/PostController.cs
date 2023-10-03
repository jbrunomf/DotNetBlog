using DotNetBlog.Data;
using DotNetBlog.Models;
using DotNetBlog.ViewModels;
using DotNetBlog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DotNetBlog.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context,
        [FromQuery]int page = 0,
        [FromQuery]int pageSize = 5)
    {
        try
        {
            var posts = await context
                .Posts
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Select(x => new ListPostViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = x.Author.Name
                })
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();
            var count = posts.Count;
            return Ok(new ResultViewModel<dynamic>(new
            {
                Total = count,
                page,
                pageSize,
                posts
            }));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet("v1/posts/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var post = await context.Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .ThenInclude(x => x.Roles)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado"));
            }

            return Ok(new ResultViewModel<Post>(post));
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<Post>("Erro ao obter postagens"));
        }
    }

    [HttpGet("v1/posts/category/{category")]
    public async Task<IActionResult> GetByCategoryAsync([FromRoute]string category,
        [FromServices]BlogDataContext context,
        [FromQuery]int page = 0,
        [FromQuery]int pageSize = 5)
    {
        try
        {
            var count = await context.Posts.AsNoTracking().CountAsync();
            var posts = await context.Posts
                .AsNoTracking()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Category.Slug == category)
                .Select(x => new ListPostViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = x.Author.Name
                }).Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.LastUpdateDate)
                .ToListAsync();
            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                page,
                pageSize,
                posts
            }));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}