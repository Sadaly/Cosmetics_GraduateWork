﻿using Domain.Common;
using Domain.Shared;

namespace Domain.Entity
{
    public class SkinFeature : TransitiveEntity<SkinFeatureType>
    {
        private SkinFeature(Guid id) : base(id) { }
        private SkinFeature(Guid id, PatientCard patientCard, SkinFeatureType type) : base(id, type)
        {
            PatientCardId = patientCard.Id;
            PatientCard = patientCard;
        }
        public PatientCard PatientCard { get; set; } = null!;
        public Guid PatientCardId { get; set; }


        public static Result<SkinFeature> Create(Result<PatientCard> patientCard, Result<SkinFeatureType> type)
        {
            if (patientCard.IsFailure) return Result.Failure<SkinFeature>(patientCard);
            if (type.IsFailure) return Result.Failure<SkinFeature>(type);

            return new SkinFeature(Guid.NewGuid(), patientCard.Value, type.Value);
        }
    }
}
