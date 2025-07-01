using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class Patient : BaseEntity
    {
        private Patient(Guid id) : base(id)
        {
            Fullname = new Username();
        }
        private Patient(Guid id, Username fullname) : base(id)
        {
            this.Fullname = fullname;
        }
        public Username Fullname { get; set; }

        public PatientCard? Card { get; set; } = null!;
        public static Result<Patient> Create(Result<Username> fullname)
        {
            if (fullname.IsFailure) return Result.Failure<Patient>(fullname.Error);
            
            var _id = Guid.NewGuid();
            var patient = new Patient(_id, fullname.Value);

            var card = PatientCard.Create(0, Text.CreateDefault(), Text.CreateDefault(), PhoneNumber.CreateDefault(), patient);
            if (card.IsFailure) return Result.Failure<Patient>(card.Error);

            patient.Card = card.Value;

            return patient;            
        }
        public Result<Patient> UpdateFullname(Result<Username> fullname)
        {
            if (fullname.IsFailure) return Result.Failure<Patient>(fullname.Error);
            if (fullname.Value == this.Fullname) return Result.Failure<Patient>(Domain.Errors.DomainErrors.Username.AlreadySet);
            this.Fullname = fullname.Value;
            return this;
        }
    }
}
