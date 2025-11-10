using System;
using System.Collections.Generic;

namespace OwlEdu_Manager_Server.Models;

public partial class ClassAssignment
{
    public string StudentId { get; set; } = null!;

    public string ClassId { get; set; } = null!;

    public DateOnly? AssignedDate { get; set; }

    public string? Status { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
