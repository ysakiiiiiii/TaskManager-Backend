using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Helpers
{
    public class ValidGuidAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string str)
                return false;

            return Guid.TryParse(str, out _);
        }
    }
}
