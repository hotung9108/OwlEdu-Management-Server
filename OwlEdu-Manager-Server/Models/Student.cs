using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OwlEdu_Manager_Server.Models;

public partial class Student
{
    public string Id { get; set; } = null!;

    public string? AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Gender { get; set; }
    //[JsonIgnore]
    public virtual Account? Account { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<ClassAssignment> ClassAssignments { get; set; } = new List<ClassAssignment>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
