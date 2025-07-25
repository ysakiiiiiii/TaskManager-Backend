﻿using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.DTOs.Category
{
    /// <summary>
    /// Response DTO for category data
    /// </summary>
    public sealed record CategoryDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
}
