using Domain.Common;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    /// <summary>
    /// Класс представляющий собой возрастные изменения пациента
    /// </summary>
    public class AgeChange : TransitiveEntity<AgeChangeType>
    {
        private AgeChange(Guid id, PatientCard patientCard, AgeChangeType type) : base(id, type)
        {
            PatientCardId = patientCard.Id;
            PatientCard = patientCard;
        }
        [JsonIgnore]
        public PatientCard PatientCard { get; set; } = null!;
        public Guid PatientCardId { get; set; }


        public static Result<AgeChange> Create(Result<PatientCard> patientCard, Result<AgeChangeType> type)
        {
            if (patientCard.IsFailure) return Result.Failure<AgeChange>(patientCard);
            if (type.IsFailure) return Result.Failure<AgeChange>(type);

            return new AgeChange(Guid.NewGuid(), patientCard.Value, type.Value);
        }
    }
}
