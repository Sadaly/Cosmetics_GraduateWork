using Domain.Entity;
using Domain.ValueObjects;

namespace Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    User.Create(Email.Create("admin@admin"), Username.Create("admin"), PasswordHashed.Create("1C6DA20E-2F30-4A9F-A70E-F4F3AD9B8583")).Value
                );
                context.SaveChanges();
            }
        }
    }
}