using System.ComponentModel.DataAnnotations;

namespace DotNetBlog.ViewModels;

public class EditorCategoryViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre {2} e {1} caracteres.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "O slug é obrigatório.")]
    public string Slug { get; set; }
}