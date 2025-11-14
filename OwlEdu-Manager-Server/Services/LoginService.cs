using Microsoft.EntityFrameworkCore;
using OwlEdu_Manager_Server.Models;

namespace OwlEdu_Manager_Server.Services
{
    public class LoginService
    {
        private readonly EnglishCenterManagementContext _context;
        private readonly JwtService _jwtService;
        public LoginService(EnglishCenterManagementContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        public async Task<(Account? Account, string? Token)> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))return (null, null);
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
            if (account == null)return (null, null);
            var token = _jwtService.GenerateToken(account.Id, account.Role ?? "User");
            return (account, token);
        }
    }
}
