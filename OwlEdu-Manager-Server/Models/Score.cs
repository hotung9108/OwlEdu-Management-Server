using System;
using System.Collections.Generic;

namespace OwlEdu_Manager_Server.Models;

public partial class Score
{
    public string? Title { get; set; }

    public string StudentId { get; set; } = null!;

    public string ClassId { get; set; } = null!;

    public string? TeacherId { get; set; }

    public decimal? Lisening { get; set; }

    public decimal? Speaking { get; set; }

    public decimal? Reading { get; set; }

    public decimal? Writing { get; set; }

    public string Type { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Teacher? Teacher { get; set; }
}
