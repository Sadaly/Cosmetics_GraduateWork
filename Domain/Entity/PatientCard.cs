using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entity
{
    public class PatientCard : BaseEntity
    {
        private const string NO_COMPLAINTS = "Жалоб нет";
        private PatientCard(Guid id) : base(id) { }

        private PatientCard(Guid id, byte age, Text adress, Text complaints, PhoneNumber phoneNumber, Patient patient) : base(id)
        {
            Age = age;
            Adress = adress;
            Complaints = complaints;
            PhoneNumber = phoneNumber;
            PatientId = patient.Id;
            Patient = patient;
        }

        public byte Age { get; set; }
        public Text Adress { get; set; } = null!;
        public Text Complaints { get; set; } = null!;
        public PhoneNumber PhoneNumber { get; set; } = null!;
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public static Result<PatientCard> Create(byte age, Result<Text> adress, Result<Text> complaints, Result<PhoneNumber> phoneNumber, Result<Patient> patient)
        {
            if (adress.IsFailure) return Result.Failure<PatientCard>(adress.Error);
            if (patient.IsFailure) return Result.Failure<PatientCard>(patient.Error);
            if (complaints.IsFailure) return Result.Failure<PatientCard>(complaints.Error);
            if (phoneNumber.IsFailure) return Result.Failure<PatientCard>(phoneNumber.Error);
            var id = Guid.NewGuid();
            return new PatientCard(id, age, adress.Value, complaints.Value, phoneNumber.Value, patient.Value);
        }

        public Result<PatientCard> Update(byte? age, Result<Text> adress, Result<Text> complaints, Result<PhoneNumber> phoneNumber)
        {
            if (adress.IsFailure) return Result.Failure<PatientCard>(adress.Error);
            if (complaints.IsFailure) return Result.Failure<PatientCard>(complaints.Error);
            if (phoneNumber.IsFailure) return Result.Failure<PatientCard>(phoneNumber.Error);

            if (age != null) Age = age.Value;
            if (adress.Value.Value != Text.DEFAULT_VALUE) Adress = adress.Value;
            if (complaints.Value.Value != Text.DEFAULT_VALUE) Complaints = complaints.Value;
            if (phoneNumber.Value.Value != PhoneNumber.DEFAULT_VALUE) PhoneNumber = phoneNumber.Value;

            return this;
        }
    }
}
