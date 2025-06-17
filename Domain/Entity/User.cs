using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class User : BaseEntity
    {
        private User(Guid id) : base(id) {
            Email = new Email();
            Username = new Username();
            PasswordHashed = new PasswordHashed();
        }
        public User(Guid id, Email email, Username username, PasswordHashed passwordHashed)
        {
            Id = id;
            Email = email;
            Username = username;
            PasswordHashed = passwordHashed;
        }

        public Email Email { get; set; }
        public Username Username { get; set; }
        public PasswordHashed PasswordHashed { get; set; }


        public static Result<User> Create(Result<Email> email, Result<Username> username, Result<PasswordHashed> passwordHashed)
        {
            if (email.IsFailure) return Result.Failure<User>(email.Error);
            if (username.IsFailure) return Result.Failure<User>(username.Error);
            if (passwordHashed.IsFailure) return Result.Failure<User>(passwordHashed.Error);
            var _id = Guid.NewGuid();

            return new User(_id, email.Value, username.Value, passwordHashed.Value);
        }
    }
}
