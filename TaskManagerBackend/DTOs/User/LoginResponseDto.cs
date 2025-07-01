namespace TaskManagerBackend.DTOs.User
{
    /// <summary>
    /// Response DTO containing authentication token
    /// </summary>
    public sealed record LoginResponseDto
    {
        public string JwtToken { get; init; }
        public DateTime? TokenExpiration { get; init; }
    }
}
