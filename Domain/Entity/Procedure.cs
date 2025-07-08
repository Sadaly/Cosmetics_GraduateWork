using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class Procedure : EntityWithTntity<ProcedureType>
    {
        private Procedure(Guid id) : base(id) { }
        private Procedure(Guid id, PatientCard patientCard, ProcedureType type, DateTime? scheduledDate, int duration, Doctor? doctor) : base(id, type)
        {
            PatientCardId = patientCard.Id;
            PatientCard = patientCard;
            ScheduledDate = scheduledDate;
            Duration = duration;
            Doctor = doctor;
            DoctorId = doctor?.Id;
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


        public static Result<Procedure> Create(Result<PatientCard> patientCard, Result<ProcedureType> type, int duration, DateTime? scheduledDate = null, Doctor? doctor = null)
        {
            if (patientCard.IsFailure) return Result.Failure<Procedure>(patientCard);
            if (type.IsFailure) return Result.Failure<Procedure>(type);
            if (duration < 0) return Result.Failure<Procedure>(DomainErrors.Procedure.DurationLessThenZero);

            return new Procedure(Guid.NewGuid(), patientCard.Value, type.Value, scheduledDate, duration, doctor);
        }

        public Result<Procedure> AssignDoctor(Result<Doctor> doctor)
        {
            if (doctor.IsFailure) Result.Failure<Procedure>(doctor);
            Doctor = doctor.Value;
            DoctorId = doctor.Value.Id;
            return this;
        }
    }
}
