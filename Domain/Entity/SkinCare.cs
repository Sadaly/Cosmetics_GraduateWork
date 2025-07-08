using Domain.Common;
using Domain.Shared;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class SkinCare : EntityWithType<SkinCareType>
    {
        private SkinCare(Guid id) : base(id) { }
        private SkinCare(Guid id, PatientCard patientCard, SkinCareType type) : base(id, type)
        {
            PatientCardId = patientCard.Id;
            PatientCard = patientCard;
        }
        [JsonIgnore]
        public PatientCard PatientCard { get; set; } = null!;
        public Guid PatientCardId { get; set; }


        public static Result<SkinCare> Create(Result<PatientCard> patientCard, Result<SkinCareType> type)
        {
            if (patientCard.IsFailure) return Result.Failure<SkinCare>(patientCard);
            if (type.IsFailure) return Result.Failure<SkinCare>(type);

            return new SkinCare(Guid.NewGuid(), patientCard.Value, type.Value);
        }
    }
}
