using System;
using System.Collections.Generic;

namespace OwlEdu_Manager_Server.Models;

public partial class Class
{
    public string Id { get; set; } = null!;

    public string? CourseId { get; set; }

    public bool Status { get; set; }

    public string? Name { get; set; }

    public decimal? Require { get; set; }

    public decimal? Target { get; set; }

    public int? MaxStudents { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? TeacherId { get; set; }

    public virtual ICollection<ClassAssignment> ClassAssignments { get; set; } = new List<ClassAssignment>();

    public virtual Course? Course { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual Teacher? Teacher { get; set; }
}
