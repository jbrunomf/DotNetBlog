﻿using System.Text.RegularExpressions;
using DotNetBlog.Data;
using DotNetBlog.ExtensionMethods;
using DotNetBlog.Models;
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
        [FromServices] BlogDataContext context,
        [FromServices] EmailService emailService)
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
            emailService.Send(
                user.Name,
                user.Email,
                "Sua conta foi criada!",
                "Bem vindo a JBSoft.",
                "JBSoft",
                "teste@envioemail.com");

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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"data:image[a-z]+;base64,").Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<string>("Erro ao salvar imagem. Tente novamente."));
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

        if (user == null)
        {
            return NotFound(new ResultViewModel<string>("Falha interna no servidor."));
        }

        return Ok(new ResultViewModel<string>("Imagem alterada com sucesso"));
    }
}