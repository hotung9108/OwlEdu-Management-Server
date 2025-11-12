using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OwlEdu_Manager_Server.Models;
public partial class Account
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Role { get; set; }

    public bool Status { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdateAt { get; set; }
    public virtual ICollection<Payment> PaymentFeeCollectors { get; set; } = new List<Payment>();
    public virtual ICollection<Payment> PaymentPayers { get; set; } = new List<Payment>();
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
