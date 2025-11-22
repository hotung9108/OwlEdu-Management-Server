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
    public class ScheduleRequest
    {
        public string? ClassId { get; set; } 
        public DateOnly? SessionDate { get; set; } 
        public TimeOnly? StartTime { get; set; } 
        public TimeOnly? EndTime { get; set; } 
        public string? Room { get; set; } 
        public string? TeacherId { get; set; } 
    }
    public class ScheduleResponse
    {
        public string Id { get; set; } = null!; 
        public string? ClassId { get; set; } 
        public string? ClassName { get; set; }
        public DateOnly? SessionDate { get; set; } 
        public TimeOnly? StartTime { get; set; } 
        public TimeOnly? EndTime { get; set; } 
        public string? Room { get; set; }
        public string? TeacherId { get; set; } 
        public string? TeacherName { get; set; } 
    }
}
