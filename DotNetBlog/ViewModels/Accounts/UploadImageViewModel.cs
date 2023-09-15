using System.ComponentModel.DataAnnotations;

namespace DotNetBlog.ViewModels;

public class UploadImageViewModel
{
    [Required(ErrorMessage = "Arquivo e imagem requerida.")]
    public string Base64Image { get; set; }
}