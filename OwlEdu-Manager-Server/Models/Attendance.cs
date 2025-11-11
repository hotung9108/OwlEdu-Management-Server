using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OwlEdu_Manager_Server.Models;

public partial class Attendance
{
    public string StudentId { get; set; } = null!;

    public string ScheduleId { get; set; } = null!;

    public string? Status { get; set; }

    public string? Note { get; set; }

    public string? TeacherId { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Teacher? Teacher { get; set; }
}
