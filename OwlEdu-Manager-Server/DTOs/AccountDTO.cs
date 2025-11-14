namespace OwlEdu_Manager_Server.DTOs
{
    //public class AccountDTO
    //{
    //    //public string Id { get; set; } = null!;
    //    public string Username { get; set; } = null!;
    //    public string Password { get; set; } = null!;
    //    public string? Avatar { get; set; }
    //    public string? Role { get; set; }
    //    public bool Status { get; set; }
    //    public string? Email { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public DateTime? UpdateAt { get; set; }
    //}
    public class AccountRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Role { get; set; }
        public bool Status { get; set; }
        public string? Email { get; set; }
        public StudentRequest? StudentRequest { get; set; }
        public TeacherRequest? TeacherRequest { get; set; }
    }
    public class AccountResponse
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Role { get; set; }
        public bool Status { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
    //public class AccountDetailResponse
    //{
    //    public string Id { get; set; } = null!;
    //    public string Username { get; set; } = null!;
    //    public string? Avatar { get; set; }
    //    public string? Role { get; set; }
    //    public bool Status { get; set; }
    //    public string? Email { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public DateTime? UpdateAt { get; set; }
    //    public List<StudentResponse>? Students { get; set; }
    //    public List<TeacherResponse>? Teachers { get; set; }
    //}
    public class AccountDetailResponse
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Role { get; set; }
        public bool Status { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public StudentResponse? Student { get; set; }
        public TeacherResponse? Teacher { get; set; }
    }
    public class AccountLoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class AccountLoginResponse
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Role { get; set; }
        public bool Status { get; set; }
        public string? Token { get; set; } 
    }
}
