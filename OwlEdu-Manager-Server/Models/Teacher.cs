using System;
using System.Collections.Generic;

namespace OwlEdu_Manager_Server.Models;

public partial class Teacher
{
    public string Id { get; set; } = null!;

    public string? AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Specialization { get; set; }

    public string? Qualification { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Gender { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
