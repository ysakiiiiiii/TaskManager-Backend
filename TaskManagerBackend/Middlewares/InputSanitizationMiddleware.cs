using System.Text.Json;
using System.Text;
using TaskManagerBackend.Helpers;
using Microsoft.Extensions.Primitives;

namespace TaskManagerBackend.Middlewares
{
    public class InputSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<InputSanitizationMiddleware> _logger;

        public InputSanitizationMiddleware(
            RequestDelegate next,
            ILogger<InputSanitizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Sanitize query string
            if (context.Request.Query.Any())
            {
                var sanitizedQuery = new Dictionary<string, StringValues>();
                foreach (var (key, value) in context.Request.Query)
                {
                    sanitizedQuery[key] = new StringValues(InputSanitizer.Sanitize(value.ToString()));
                }
                context.Request.Query = new QueryCollection(sanitizedQuery);
            }

            // Sanitize form data
            if (context.Request.HasFormContentType)
            {
                var form = await context.Request.ReadFormAsync();
                var sanitizedForm = new Dictionary<string, StringValues>();
                foreach (var (key, value) in form)
                {
                    sanitizedForm[key] = new StringValues(InputSanitizer.Sanitize(value.ToString()));
                }
                context.Request.Form = new FormCollection(sanitizedForm);
            }

            // Sanitize route values
            foreach (var (key, value) in context.Request.RouteValues)
            {
                if (value is string stringValue)
                {
                    context.Request.RouteValues[key] = InputSanitizer.Sanitize(stringValue);
                }
            }

            // Sanitize JSON body
            if (context.Request.ContentType?.Contains("application/json") == true)
            {
                context.Request.EnableBuffering();
                var originalBody = context.Request.Body;

                try
                {
                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    var json = await reader.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var sanitizedJson = SanitizeJson(json);
                        var bytes = Encoding.UTF8.GetBytes(sanitizedJson);
                        context.Request.Body = new MemoryStream(bytes);
                        context.Request.ContentLength = bytes.Length;
                    }
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }
                finally
                {
                    originalBody.Dispose();
                }
            }

            await _next(context);
        }

        private string SanitizeJson(string json)
        {
            using var document = JsonDocument.Parse(json);
            var sanitizedElement = SanitizeElement(document.RootElement);
            return JsonSerializer.Serialize(sanitizedElement);
        }

        private object SanitizeElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var sanitizedObject = new Dictionary<string, object>();
                    foreach (var property in element.EnumerateObject())
                    {
                        sanitizedObject[property.Name] = SanitizeElement(property.Value);
                    }
                    return sanitizedObject;

                case JsonValueKind.Array:
                    var sanitizedArray = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        sanitizedArray.Add(SanitizeElement(item));
                    }
                    return sanitizedArray;

                case JsonValueKind.String:
                    return InputSanitizer.Sanitize(element.GetString());

                case JsonValueKind.Number:
                    return element.TryGetInt64(out var longVal) ? longVal : element.GetDouble();

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();

                case JsonValueKind.Null:
                default:
                    return null;
            }
        }


    }
}
