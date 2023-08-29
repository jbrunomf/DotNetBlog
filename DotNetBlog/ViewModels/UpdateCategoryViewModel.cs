using Blog.Models;

namespace DotNetBlog.ViewModels;

public class UpdateCategoryViewModel
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public IList<Post> Posts { get; set; }
}