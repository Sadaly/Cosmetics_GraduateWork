using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
	public class ReservedDateFilter : IEntityFilter<ReservedDate>
	{
		public string? Type { get; set; }
		public DateTime? CreationDateFrom { get; set; }
		public DateTime? CreationDateTo { get; set; }

		public Expression<Func<ReservedDate, bool>> ToPredicate()
		{
			return reserveddate =>
				(string.IsNullOrWhiteSpace(Type) || reserveddate.Type.ToString().Contains(Type)) &&
				(!CreationDateFrom.HasValue || reserveddate.CreatedAt >= CreationDateFrom.Value) &&
				(!CreationDateTo.HasValue || reserveddate.CreatedAt <= CreationDateTo.Value);
		}
	}
}
