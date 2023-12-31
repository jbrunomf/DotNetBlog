﻿using DotNetBlog.Data;
using DotNetBlog.ExtensionMethods;
using DotNetBlog.Models;
using DotNetBlog.ViewModels;
using DotNetBlog.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetBlog.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    // [Authorize]
    [HttpGet("v1/categories")]
    public IActionResult GetAsync(
        [FromServices] BlogDataContext context,
        [FromServices] IMemoryCache cache)
    {
        try
        {
            var categories = cache.GetOrCreate("CategoriesCache", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return GetCategories(context);
            });
            return Ok(new ResultViewModel<List<Category>>(categories));
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<string>("Erro ao obter categorias."));
        }
    }

    private List<Category> GetCategories(BlogDataContext context)
    {
        return context.Categories.ToList();
    }

    [HttpGet("v1/categories/{id:int}")]
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

            return Ok(new ResultViewModel<Category>(category));
        }
        catch (Exception e)
        {
            return BadRequest(new ResultViewModel<string>("Erro ao obter categoria."));
        }
    }

    [HttpPost("v1/categories")]
    public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResultViewModel<List<string>>(ModelState.GetErrors()));
        }

        try
        {
            var category = new Category
            {
                Id = 0,
                Name = model.Name,
                Slug = model.Slug
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return Created($"v1/categories/{category.Id}", category);
        }
        catch (DbUpdateException e)
        {
            return StatusCode(500, new ResultViewModel<string>("Não foi possível incluir a categoria."));
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor."));
        }
    }

    [HttpPut("v1/categories/{id:int}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id,
        [FromBody] EditorCategoryViewModel model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound(new ResultViewModel<string>("Categoria não encontrada."));

            category.Name = model.Name;
            category.Slug = model.Slug;
            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Category>(category));
        }
        catch (Exception e)
        {
            return BadRequest(new ResultViewModel<string>("Falha interna no servidor."));
        }
    }

    [HttpDelete("v1/categories/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound(new ResultViewModel<string>("Categoria não encontrada."));
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new ResultViewModel<Category>(category));
        }
        catch (Exception e)
        {
            return BadRequest(new ResultViewModel<string>("Falha interna do servidor"));
        }
    }
}