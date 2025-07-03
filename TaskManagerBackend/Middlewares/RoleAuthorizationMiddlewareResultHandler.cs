using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace TaskManagerBackend.Middlewares
{
    public class RoleAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    error = "You are not authorized to access this resource."
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
                return;
            }

            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    error = "Authentication is required."
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
                return;
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
