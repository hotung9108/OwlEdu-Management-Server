using System;
using System.Collections.Generic;

namespace OwlEdu_Manager_Server.Models;

public partial class Schedule
{
    public string Id { get; set; } = null!;

    public string? ClassId { get; set; }

    public DateOnly? SessionDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Room { get; set; }

    public string? TeacherId { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Class? Class { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
