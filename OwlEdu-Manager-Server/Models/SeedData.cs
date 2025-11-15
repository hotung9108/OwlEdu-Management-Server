namespace OwlEdu_Manager_Server.Models
{
    public static class SeedData
    {
        public static async Task InitializeAsync(EnglishCenterManagementContext db)
        {
            if (db.Accounts.Any()) return; // Chỉ chạy 1 lần

            var users = new[]
            {
            new Account { Id = "U000000001", Username = "admin01", Password = "123456", Avatar = null, Role = "admin", Status = true, Email = "admin01@center.com" },
            new Account { Id = "U000000002", Username = "student01", Password = "123456", Avatar = null, Role = "student", Status = true, Email = "student01@gmail.com" },
            new Account { Id = "U000000003", Username = "student02", Password = "123456", Avatar = null, Role = "student", Status = true, Email = "student02@gmail.com" },
            new Account { Id = "U000000004", Username = "teacher01", Password = "123456", Avatar = null, Role = "teacher", Status = true, Email = "teacher01@center.com" },
            new Account { Id = "U000000005", Username = "teacher02", Password = "123456", Avatar = null, Role = "teacher", Status = true, Email = "teacher02@center.com" },
            };
            db.Accounts.AddRange(users);
            await db.SaveChangesAsync();
        }
    }
}
