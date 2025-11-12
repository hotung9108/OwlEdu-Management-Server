using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.DTOs
{
    public class ClassAssignmentDTO
    {
        public string StudentId { get; set; } = null!;

        public string ClassId { get; set; } = null!;

        public DateOnly? AssignedDate { get; set; }

        public string? Status { get; set; }
    }
}
