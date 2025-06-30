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
        private User(Guid id, Email email, Username username, PasswordHashed passwordHashed) : base(id) {
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

        public Result<User> UpdateEmail(Result<Email> email)
        {
            if (email.IsFailure) return Result.Failure<User>(email.Error);
            if (email.Value == this.Email) return Result.Failure<User>(Domain.Errors.DomainErrors.Email.AlreadySet);
            this.Email = email.Value;
            this.UpdatedAt = DateTime.UtcNow;
            return this;
        }
        public Result<User> UpdateUsername(Result<Username> username)
        {
            if (username.IsFailure) return Result.Failure<User>(username.Error);
            if (username.Value == this.Username) return Result.Failure<User>(Domain.Errors.DomainErrors.Username.AlreadySet);
            this.Username = username.Value;
            this.UpdatedAt = DateTime.UtcNow;
            return this;
        }
        public Result<User> UpdatePassword(Result<PasswordHashed> passwordHashed)
        {
            if (passwordHashed.IsFailure) return Result.Failure<User>(passwordHashed.Error);
            if (passwordHashed.Value == this.PasswordHashed) return Result.Failure<User>(Domain.Errors.DomainErrors.PasswordHashed.AlreadySet);
            this.PasswordHashed = passwordHashed.Value;
            this.UpdatedAt = DateTime.UtcNow;
            return this;
        }
    }
}
