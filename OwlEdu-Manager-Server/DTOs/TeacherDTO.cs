namespace OwlEdu_Manager_Server.DTOs
{
    public class TeacherRequest
    {
        public string FullName { get; set; } = null!;
        public string? Specialization { get; set; }
        public string? Qualification { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
    }
    public class TeacherResponse
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Specialization { get; set; }
        public string? Qualification { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
    }
    public class TeacherDetailResponse
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Specialization { get; set; }
        public string? Qualification { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public AccountResponse? accountResponse { get; set; }
    }
}
