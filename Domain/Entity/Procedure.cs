using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
	public class Procedure : EntityWithType<ProcedureType>
	{
		private Procedure(Guid id) : base(id) { }
		private Procedure(Guid id, PatientCard patientCard, ProcedureType type, DateTime? scheduledDate, int duration, int price, Doctor? doctor) : base(id, type)
		{
			PatientCardId = patientCard.Id;
			PatientCard = patientCard;
			ScheduledDate = scheduledDate;
			Duration = duration;
			Doctor = doctor;
			DoctorId = doctor?.Id;
			Price = price;
		}
		[JsonIgnore]
		public PatientCard PatientCard { get; set; } = null!;
		public Guid PatientCardId { get; set; }
		public DateTime? ScheduledDate { get; set; }
		public int Duration { get; set; }
		[NotMapped]
		public DateTime? EndDateTime => ScheduledDate?.Add(TimeSpan.FromMinutes(Duration));

		public Guid? DoctorId { get; set; }
		public Doctor? Doctor { get; set; }
		public Notification? Notification { get; set; }
		public bool IsComplete { get; set; } = false;
		public bool IsPostponded { get; set; } = false;
		public bool IsCancelled { get; set; } = false;
		public int Price { get; set; }
		public List<Resource> UsedResources { get; set; } = [];

		public static Result<Procedure> Create(Result<PatientCard> patientCard, Result<ProcedureType> type, int duration, int price, DateTime? scheduledDate = null, Doctor? doctor = null)
		{
			if (patientCard.IsFailure) return patientCard.Error;
			if (type.IsFailure) return type.Error;
			if (duration < 0) return DomainErrors.Procedure.DurationLessThenZero;
			if (price < 0) return DomainErrors.Procedure.PriceLessThenZero;

			return new Procedure(Guid.NewGuid(), patientCard.Value, type.Value, scheduledDate, duration , price, doctor);
		}

		public Result<Procedure> AssignDoctor(Result<Doctor> doctor)
		{
			if (doctor.IsFailure) return doctor.Error;
			Doctor = doctor.Value;
			DoctorId = doctor.Value.Id;
			return this;
		}
		public Result<Procedure> RemoveDoctor()
		{
			if (DoctorId == null) return DomainErrors.Procedure.AlreadyNoDoctor;
			Doctor = null;
			DoctorId = null;
			return this;
		}
		public Result<Procedure> UpdateDate(DateTime scheduledDate, int duration)
		{
			ScheduledDate = scheduledDate;
			Duration = duration;
			return this;
		}
	}
}
