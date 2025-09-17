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

		public List<AgeChange> AgeChanges { get; } = [];
		public List<ExternalProcedureRecord> ExternalProcedureRecords { get; } = [];
		public List<HealthCond> HealthConds { get; } = [];

		public List<Procedure> Procedures { get; } = [];
		public List<SkinCare> SkinCares { get; } = [];
		public List<SkinFeature> SkinFeatures { get; } = [];

		public static Result<PatientCard> Create(byte age, Result<Text> adress, Result<Text> complaints, Result<PhoneNumber> phoneNumber, Result<Patient> patient)
		{
			if (adress.IsFailure) return adress.Error;
			if (patient.IsFailure) return patient.Error;
			if (complaints.IsFailure) return complaints.Error;
			if (phoneNumber.IsFailure) return phoneNumber.Error;
			var id = Guid.NewGuid();
			return new PatientCard(id, age, adress.Value, complaints.Value, phoneNumber.Value, patient.Value);
		}

		public Result<PatientCard> Update(byte? age, Result<Text> adress, Result<Text> complaints, Result<PhoneNumber> phoneNumber)
		{
			if (adress.IsFailure) return adress.Error;
			if (complaints.IsFailure) return complaints.Error;
			if (phoneNumber.IsFailure) return phoneNumber.Error;

			if (age != null) Age = age.Value;
			if (adress.Value.Value != Text.DEFAULT_VALUE) Adress = adress.Value;
			if (complaints.Value.Value != Text.DEFAULT_VALUE) Complaints = complaints.Value;
			if (phoneNumber.Value.Value != PhoneNumber.DEFAULT_VALUE) PhoneNumber = phoneNumber.Value;

			return this;
		}
	}
}
