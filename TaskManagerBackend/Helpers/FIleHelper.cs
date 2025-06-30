using Microsoft.AspNetCore.StaticFiles;

namespace TaskManagerBackend.Helpers
{
    public static class FileHelper
    {
        public static string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream"; 
            }

            return contentType;
        }
    }
}
