
using Application.Abstractions.Messaging;

namespace Application.Entity.PatientCards.Queries.GetByPatientId;
public sealed record PatientCardGetByPatientIdQuery(Guid PatientId) : IQuery<PatientCardResponse>;

