namespace OwlEdu_Manager_Server.DTOs
{
    public class EnrollmentDTO
    {
        public string Id { get; set; } = null!;

        public string? StudentId { get; set; }

        public string? CourseId { get; set; }

        public DateOnly? EnrollmentDate { get; set; }

        public string? Status { get; set; }

        public string? CreatedBy { get; set; }
    }
}
