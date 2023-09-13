using Blog.Data;
using Blog.Models;
using DotNetBlog.ExtensionMethods;
using DotNetBlog.Services;
using DotNetBlog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace DotNetBlog.Controllers;

[ApiController]
[Route("v1")]
public class AccountController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromServices] TokenService tokenService)
    {
        var token = tokenService.GenerateToken(null);
        return Ok(token);
    }

    [HttpPost("accounts")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        }

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-")
        };
        var password = PasswordGenerator.Generate(25);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email,
                password
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("Este e-mail já está sendo utilizado."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Falha ao cadastrar usuário."));
        }
        
    }
}