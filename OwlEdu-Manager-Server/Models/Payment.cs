using System;
using System.Collections.Generic;

namespace OwlEdu_Manager_Server.Models;

public partial class Payment
{
    public string Id { get; set; } = null!;

    public string? EnrollmentId { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public string? FeeCollectorId { get; set; }

    public string? PayerId { get; set; }

    public string? Method { get; set; }

    public string? Status { get; set; }

    public virtual Enrollment? Enrollment { get; set; }

    public virtual Account? FeeCollector { get; set; }

    public virtual Account? Payer { get; set; }
}
