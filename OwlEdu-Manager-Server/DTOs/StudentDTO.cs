namespace OwlEdu_Manager_Server.DTOs
{
    public class StudentRequest
    {
        public string FullName { get; set; } = null!;
        public DateOnly? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
    }
    public class StudentResponse
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
    }
    public class StudentDetailResponse
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateOnly? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public AccountResponse? accountResponse { get; set; }
    }
}
