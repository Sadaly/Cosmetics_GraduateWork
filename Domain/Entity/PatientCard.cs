using Domain.Common;
using Domain.Shared;
using Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
	public class PatientCard : BaseEntity
	{
		private PatientCard(Guid id) : base(id) { }

		private PatientCard(Guid id, byte age, Text address, Text complaints, PhoneNumber phoneNumber, Patient patient) : base(id)
		{
			Age = age;
			Address = address;
			Complaints = complaints;
			PhoneNumber = phoneNumber;
			PatientId = patient.Id;
			Patient = patient;
		}

		public byte Age { get; set; }
		public Text Address { get; set; } = null!;
		public Text Complaints { get; set; } = null!;
		public PhoneNumber PhoneNumber { get; set; } = null!;
		public Guid PatientId { get; set; }
		[JsonIgnore]
		public Patient Patient { get; set; } = null!;
		public PatientSpecifics? Specifics { get; set; } = null!;

		public List<AgeChange> AgeChanges { get; } = [];
		public List<ExternalProcedureRecord> ExternalProcedureRecords { get; } = [];
		public List<HealthCond> HealthConds { get; } = [];

		public List<Procedure> Procedures { get; } = [];
		public List<SkinCare> SkinCares { get; } = [];
		public List<SkinFeature> SkinFeatures { get; } = [];

		public static Result<PatientCard> Create(byte age, Result<Text> address, Result<Text> complaints, Result<PhoneNumber> phoneNumber, Result<Patient> patient)
		{
			if (address.IsFailure) return address.Error;
			if (patient.IsFailure) return patient.Error;
			if (complaints.IsFailure) return complaints.Error;
			if (phoneNumber.IsFailure) return phoneNumber.Error;
			var id = Guid.NewGuid();
			return new PatientCard(id, age, address.Value, complaints.Value, phoneNumber.Value, patient.Value);
		}

		public Result<PatientCard> Update(byte? age, Result<Text> address, Result<Text> complaints, Result<PhoneNumber> phoneNumber)
		{
			if (address.IsFailure) return address.Error;
			if (complaints.IsFailure) return complaints.Error;
			if (phoneNumber.IsFailure) return phoneNumber.Error;

			if (age != null) Age = age.Value;
			if (address.Value.Value != Text.DEFAULT_VALUE) Address = address.Value;
			if (complaints.Value.Value != Text.DEFAULT_VALUE) Complaints = complaints.Value;
			if (phoneNumber.Value.Value != PhoneNumber.DEFAULT_VALUE) PhoneNumber = phoneNumber.Value;

			return this;
		}
	}
}
