using Application.Abstractions;
using Application.Abstractions.Messaging;
using Domain.Entity;

namespace Application.Entity.ReservedDates.Queries.GetAll;
public sealed record ReservedDateGetAllQuery(
	EntityQueries<ReservedDate> Query,
	int? StartIndex = null,
	int? Count = null) : IQuery<List<ReservedDateResponse>>;