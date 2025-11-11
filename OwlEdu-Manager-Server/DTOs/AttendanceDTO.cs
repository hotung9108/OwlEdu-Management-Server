namespace OwlEdu_Manager_Server.DTOs
{
    public class AttendanceDTO
    {
        public string StudentId { get; set; } = null!;

        public string ScheduleId { get; set; } = null!;

        public string? Status { get; set; }

        public string? Note { get; set; }

        public string? TeacherId { get; set; }
    }
}
