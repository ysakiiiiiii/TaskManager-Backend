using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Helpers
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is null) return true;

            if (value is DateTime date)
            {
                return date > DateTime.Now;
            }

            return false;
        }
    }
}