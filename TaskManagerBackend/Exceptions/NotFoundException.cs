using Microsoft.AspNetCore.Http;

namespace TaskManagerBackend.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound)
        {
        }
    }
}
