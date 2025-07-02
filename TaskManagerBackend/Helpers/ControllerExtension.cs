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

        public static IActionResult ValidateRequest(this ControllerBase controller, object dto, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return controller.Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
            }

            if (!controller.ModelState.IsValid)
            {
                var errors = controller.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return controller.BadRequest(ApiResponse.ErrorResponse("Invalid data", errors));
            }

            if (dto == null)
            {
                return controller.BadRequest(ApiResponse.ErrorResponse("Request body cannot be null"));
            }

            return controller.Ok();
        }

        public static IActionResult ValidateRequest(this ControllerBase controller, string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return controller.Unauthorized(ApiResponse.ErrorResponse("User not authenticated"));
            }

            if (!controller.ModelState.IsValid)
            {
                var errors = controller.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return controller.BadRequest(ApiResponse.ErrorResponse("Invalid data", errors));
            }
            return controller.Ok();
        }
    }

}
