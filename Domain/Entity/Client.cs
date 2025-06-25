using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class Client : BaseEntity
    {
        private Client(Guid id) : base(id) {
            Email = new Email();
            Username = new Username();
            PasswordHashed = new PasswordHashed();
        }
        public Client(Guid id, Email email, Username username, PasswordHashed passwordHashed, PhoneNumber? phoneNumber)
        {
            Id = id;
            Email = email;
            Username = username;
            PasswordHashed = passwordHashed;
            PhoneNumber = phoneNumber;
        }

        public Email Email { get; set; }
        public Username Username { get; set; }
        public PasswordHashed PasswordHashed { get; set; }
        public PhoneNumber? PhoneNumber { get; set; }


        public static Result<Client> Create(Result<Email> email, Result<Username> username, Result<PasswordHashed> passwordHashed, Result<PhoneNumber?> phoneNumber)
        {
            if (email.IsFailure) return Result.Failure<Client>(email.Error);
            if (username.IsFailure) return Result.Failure<Client>(username.Error);
            if (passwordHashed.IsFailure) return Result.Failure<Client>(passwordHashed.Error);
            var _id = Guid.NewGuid();

            return new Client(_id, email.Value, username.Value, passwordHashed.Value, phoneNumber.Value);
        }
    }
}
