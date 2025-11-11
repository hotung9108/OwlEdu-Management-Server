namespace OwlEdu_Manager_Server.DTOs
{
    public class CourseDTO
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool Status { get; set; }

        public int? NumberOfLessons { get; set; }

        public decimal? Fee { get; set; }
    }
}
