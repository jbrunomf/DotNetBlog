using System.ComponentModel.DataAnnotations;

namespace DotNetBlog.ViewModels.Categories;

public class EditorCategoryViewModel
{
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "{0} deve conter entre {2} e {1} caracteres.")]
    [Display(Name = "Nome")]
    public string Name { get; set; }
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    public string Slug { get; set; }
}