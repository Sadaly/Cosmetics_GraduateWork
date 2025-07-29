using Domain.Entity;
using Domain.ValueObjects;
using System.Reflection;
using System.Text.Json;

namespace Persistence
{
    public static class DbInitializer
    {
        private static string _file = "first-admin-login.json";
        private class Admin
        {
            public string Email { get; set; } = null!;
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
        }
        public static void Initialize(AppDbContext context)
        {
            if (IsTestEnvironment())
                return;

            if (!context.Users.Any())
            {
                string json = File.ReadAllText(_file);
                var admin = JsonSerializer.Deserialize<Admin>(json);

                if (admin == null) throw new Exception("Невозможно создать первого пользователя системы");

                context.Users.AddRange(
                    User.Create(Email.Create(admin.Email), Username.Create(admin.Username), PasswordHashed.Create(admin.Password)).Value
                );
                context.SaveChanges();
            }
        }
        private static bool IsTestEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test" ||
                   Assembly.GetEntryAssembly() == null ||
                   AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName != null && a.FullName.Contains("Test"));
        }
    }
}