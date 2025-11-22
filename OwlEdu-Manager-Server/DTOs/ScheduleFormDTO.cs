using Microsoft.AspNetCore.Mvc;

namespace OwlEdu_Manager_Server.DTOs
{
    public class ScheduleFormDTO
    {
        public string ClassId { get; set; }
        public string Room { get; set; }
        public string DaysOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

    }
}
