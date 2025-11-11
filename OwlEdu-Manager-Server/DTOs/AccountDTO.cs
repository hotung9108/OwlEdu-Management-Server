namespace OwlEdu_Manager_Server.DTOs
{
    public class AccountDTO
    {
        public string Id { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? Avatar { get; set; }

        public string? Role { get; set; }

        public bool Status { get; set; }

        public string? Email { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdateAt { get; set; }

    }
}
