namespace OwlEdu_Manager_Server.DTOs
{
    public class StudentDTO
    {
        public string Id { get; set; } = null!;

        public string? AccountId { get; set; }

        public string FullName { get; set; } = null!;

        public DateOnly? BirthDate { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }
    }
}
