using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class AgeChangeTypeFilter : IEntityFilter<AgeChangeType>
    {
        public string? Typename { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<AgeChangeType, bool>> ToPredicate()
        {
            return agechangetype =>
                (string.IsNullOrWhiteSpace(Typename) || agechangetype.Title.Value.Contains(Typename)) &&
                (!CreationDateFrom.HasValue || agechangetype.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || agechangetype.CreatedAt <= CreationDateTo.Value);
        }
    }
}
