using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManagerBackend.Helpers
{
    public static class ControllerExtensions
    {
        public static IActionResult HandleResponse(this ControllerBase controller, ApiResponse response)
        {
            if (response.Success)
            {
                return controller.Ok(response);
            }

            if (response.Errors != null && response.Errors.Any())
            {
                return controller.BadRequest(response);
            }

            return controller.BadRequest(new { response.Message });
        }

        public static string GetUserId(this ControllerBase controller)
        {
            return controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
