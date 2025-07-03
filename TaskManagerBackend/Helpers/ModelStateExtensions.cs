using Microsoft.AspNetCore.Mvc.ModelBinding;

public static class ModelStateExtensions
{
    public static List<string> ToErrorList(this ModelStateDictionary modelState)
    {
        return modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
    }
}
