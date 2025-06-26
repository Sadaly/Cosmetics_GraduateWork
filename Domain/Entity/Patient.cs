using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class Patient : BaseEntity
    {
        private Patient(Guid id) : base(id)
        {
            Email = new Email();
            PhoneNumber = new PhoneNumber();
            Username = new Username();
        }
        private Patient(Guid Id, Email? email, Username username, PhoneNumber phoneNumber) : base(Id)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            Username = username;
        }
        public Email? Email { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public Username Username { get; set; }

        public static Result<Patient> Create(Result<Username> username, Result<PhoneNumber> phoneNumber, Result<Email>? email = null)
        {
            if (email != null && email.IsFailure) return Result.Failure<Patient>(email.Error);
            if (username.IsFailure) return Result.Failure<Patient>(username.Error);
            if (phoneNumber.IsFailure) return Result.Failure<Patient>(phoneNumber.Error);
            var _id = Guid.NewGuid();

            return new Patient(_id, email?.Value, username.Value, phoneNumber.Value);
        }
    }
}
