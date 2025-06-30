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

        public Result<Patient> UpdateEmail(Result<Email> email)
        {
            if (email.IsFailure) return Result.Failure<Patient>(email.Error);
            if (email.Value == this.Email) return Result.Failure<Patient>(Domain.Errors.DomainErrors.Email.AlreadySet);
            this.Email = email.Value;
            return this;
        }
        public Result<Patient> UpdateUsername(Result<Username> username)
        {
            if (username.IsFailure) return Result.Failure<Patient>(username.Error);
            if (username.Value == this.Username) return Result.Failure<Patient>(Domain.Errors.DomainErrors.Username.AlreadySet);
            this.Username = username.Value;
            return this;
        }
        public Result<Patient> UpdatePassword(Result<PhoneNumber> phoneNumber)
        {
            if (phoneNumber.IsFailure) return Result.Failure<Patient>(phoneNumber.Error);
            if (phoneNumber.Value == this.PhoneNumber) return Result.Failure<Patient>(Domain.Errors.DomainErrors.PhoneNumber.AlreadySet);
            this.PhoneNumber = phoneNumber.Value;
            return this;
        }
    }
}
