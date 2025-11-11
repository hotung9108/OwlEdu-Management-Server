namespace OwlEdu_Manager_Server.DTOs
{
    public class ScheduleDTO
    {
        public string Id { get; set; } = null!;

        public string? ClassId { get; set; }

        public DateOnly? SessionDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public string? Room { get; set; }

        public string? TeacherId { get; set; }
    }
}
