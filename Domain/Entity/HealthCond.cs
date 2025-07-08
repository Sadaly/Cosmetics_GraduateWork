using Domain.Common;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class HealthCond : TransitiveEntity<HealthCondType>
    {
        private HealthCond(Guid id) : base(id) { }
        private HealthCond(Guid id, PatientCard patientCard, HealthCondType type) : base(id, type)
        {
            PatientCardId = patientCard.Id;
            PatientCard = patientCard;
        }
        [JsonIgnore]
        public PatientCard PatientCard { get; set; } = null!;
        public Guid PatientCardId { get; set; }


        public static Result<HealthCond> Create(Result<PatientCard> patientCard, Result<HealthCondType> type)
        {
            if (patientCard.IsFailure) return Result.Failure<HealthCond>(patientCard);
            if (type.IsFailure) return Result.Failure<HealthCond>(type);

            return new HealthCond(Guid.NewGuid(), patientCard.Value, type.Value);
        }
    }
}
