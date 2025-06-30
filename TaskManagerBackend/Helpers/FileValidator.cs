namespace TaskManagerBackend.Helpers
{
    public static class FileValidator
    {
        private static readonly Dictionary<string, byte[]> FileSignatures = new()
        {
            // Image formats
            [".jpg"] = new byte[] { 0xFF, 0xD8, 0xFF },
            [".png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47 },

            // PDF
            [".pdf"] = new byte[] { 0x25, 0x50, 0x44, 0x46 },

            // DOCX, XLSX, PPTX
            [".docx"] = new byte[] { 0x50, 0x4B, 0x03, 0x04 },
            [".xlsx"] = new byte[] { 0x50, 0x4B, 0x03, 0x04 },
            [".pptx"] = new byte[] { 0x50, 0x4B, 0x03, 0x04 }
        };

        public static async Task<bool> IsValidFileAsync(IFormFile file, string extension)
        {
            if (!FileSignatures.ContainsKey(extension)) return false;

            var expectedSignature = FileSignatures[extension];
            byte[] actualBytes = new byte[expectedSignature.Length];

            using var stream = file.OpenReadStream();
            await stream.ReadAsync(actualBytes, 0, actualBytes.Length);

            return actualBytes.SequenceEqual(expectedSignature);
        }
    }

}
