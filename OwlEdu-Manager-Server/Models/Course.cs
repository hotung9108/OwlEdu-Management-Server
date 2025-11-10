using System;
using System.Collections.Generic;

namespace OwlEdu_Manager_Server.Models;

public partial class Course
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool Status { get; set; }

    public int? NumberOfLessons { get; set; }

    public decimal? Fee { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
