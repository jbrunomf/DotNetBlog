using System.ComponentModel.DataAnnotations;

namespace DotNetBlog.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    public string  Name { get; set; }
    
    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Senha é obrigatório.")]
    public string Senha { get; set; }
}