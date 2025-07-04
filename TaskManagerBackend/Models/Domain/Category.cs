﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Models.Domain
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

    }
}
