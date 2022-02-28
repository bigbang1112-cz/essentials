﻿using System.ComponentModel.DataAnnotations;

namespace BigBang1112.Models.Db;

public class TwitterAuthModel
{
    public int Id { get; set; }

    [Required]
    public ulong UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public virtual AccountModel? Account { get; set; }
}
