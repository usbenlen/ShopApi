using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shop.Domain.Models;

public abstract class BaseEntity
{
    /// <summary>
    /// Коли був створений
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Коли був оновлений
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
