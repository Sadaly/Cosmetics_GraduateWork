using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class PatientCard : BaseEntity
    {
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
        [JsonIgnore]
        public Patient Patient { get; set; } = null!;
        public PatientSpecifics? Specifics { get; set; } = null!;

        public List<AgeChange> AgeChanges => _ageChanges;
        private readonly List<AgeChange> _ageChanges = [];
        public List<ExternalProcedureRecord> ExternalProcedureRecords => _externalProcedureRecords;
        private readonly List<ExternalProcedureRecord> _externalProcedureRecords = [];
        public List<HealthCond> HealthConds => _healthConds;
        private readonly List<HealthCond> _healthConds = [];
        public List<Procedure> Procedures => _procedures;
        private readonly List<Procedure> _procedures = [];
        public List<SkinCare> SkinCares => _skinCares;
        private readonly List<SkinCare> _skinCares = [];
        public List<SkinFeature> SkinFeatures => _skinFeatures;
        private readonly List<SkinFeature> _skinFeatures = [];

        internal static Result<PatientCard> Create(byte age, Result<Text> adress, Result<Text> complaints, Result<PhoneNumber> phoneNumber, Result<Patient> patient)
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
