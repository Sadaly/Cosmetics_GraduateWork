using Domain.Common;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class ExternalProcedureRecord : EntityWithType<ExternalProcedureRecordType>
    {
        private ExternalProcedureRecord(Guid id) : base(id) { }
        private ExternalProcedureRecord(Guid id, PatientCard patientCard, ExternalProcedureRecordType type, DateOnly? dateOnly) : base(id, type)
        {
            PatientCardId = patientCard.Id;
            PatientCard = patientCard;
            DateOnly = dateOnly;
        }
        [JsonIgnore]
        public PatientCard PatientCard { get; set; } = null!;
        public Guid PatientCardId { get; set; }
        public DateOnly? DateOnly { get; set; }


        public static Result<ExternalProcedureRecord> Create(Result<PatientCard> patientCard, Result<ExternalProcedureRecordType> type, DateOnly? dateOnly)
        {
            if (patientCard.IsFailure) return Result.Failure<ExternalProcedureRecord>(patientCard);
            if (type.IsFailure) return Result.Failure<ExternalProcedureRecord>(type);

            return new ExternalProcedureRecord(Guid.NewGuid(), patientCard.Value, type.Value, dateOnly);
        }
    }
}
