namespace OwlEdu_Manager_Server.DTOs
{
    public class ClassDTO
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
    }
}
