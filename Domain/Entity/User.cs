using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class User : BaseEntity
    {
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


        public static Result<User> Create(string email, string username, string passwordHashed)
        {
            var _email = Email.Create(email); 
                if (_email.IsFailure) return Result.Failure<User>(_email.Error);
            var _username = Username.Create(username);
                if (_username.IsFailure) return Result.Failure<User>(_username.Error);
            var _passwordHash = PasswordHashed.Create(passwordHashed);
                if (_passwordHash.IsFailure) return Result.Failure<User>(_passwordHash.Error);
            var _id = Guid.NewGuid();

            return new User(_id, _email.Value, _username.Value, _passwordHash.Value);
        }
    }
}
