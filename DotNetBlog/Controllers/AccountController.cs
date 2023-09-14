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
    public async Task<IActionResult> Login([FromBody] LoginViewModel model,
        [FromServices] BlogDataContext context,
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        }

        var user = await context
            .Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user == null)
        {
            return StatusCode(401, new ResultViewModel<string>("Usuário inexistente."));
        }

        if (!PasswordHasher.Verify(user.PasswordHash, model.Senha))
        {
            return BadRequest("Usuário / senha inválido(s).");
        }

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna do servidor."));
        }
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
                password,
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