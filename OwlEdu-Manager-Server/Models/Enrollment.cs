using System;
using System.Collections.Generic;

namespace OwlEdu_Manager_Server.Models;

public partial class Enrollment
{
    public string Id { get; set; } = null!;

    public string? StudentId { get; set; }

    public string? CourseId { get; set; }

    public DateOnly? EnrollmentDate { get; set; }

    public string? Status { get; set; }

    public string? CreatedBy { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Student? Student { get; set; }
}
