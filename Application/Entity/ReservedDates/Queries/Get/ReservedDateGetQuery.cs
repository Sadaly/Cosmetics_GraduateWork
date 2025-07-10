using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.ReservedDates.Queries.Get;
public sealed record ReservedDateGetQuery(EntityQueries<ReservedDate> Query) : IQuery<ReservedDateResponse>;
