using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetBlog.ExtensionMethods;

public static class ModelStateExtension
{
    public static List<string> GetErrors(this ModelStateDictionary modelStateDictionary)
    {
        var result = new List<string>();
        foreach (var item in modelStateDictionary.Values)
            result.AddRange(item.Errors.Select(error => error.ErrorMessage));
        return result;
    }
}