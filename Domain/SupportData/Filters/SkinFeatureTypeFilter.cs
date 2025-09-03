using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
	public class SkinFeatureTypeFilter : IEntityFilter<SkinFeatureType>
	{
		public string? Typename { get; set; }
		public DateTime? CreationDateFrom { get; set; }
		public DateTime? CreationDateTo { get; set; }

		public Expression<Func<SkinFeatureType, bool>> ToPredicate()
		{
			return skinfeaturetype =>
				(string.IsNullOrWhiteSpace(Typename) || skinfeaturetype.Title.Value.Contains(Typename)) &&
				(!CreationDateFrom.HasValue || skinfeaturetype.CreatedAt >= CreationDateFrom.Value) &&
				(!CreationDateTo.HasValue || skinfeaturetype.CreatedAt <= CreationDateTo.Value);
		}
	}
}
