using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class HealthCondTypeFilter : IEntityFilter<HealthCondType>
    {
        public string? Typename { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<HealthCondType, bool>> ToPredicate()
        {
            return healthcondtype =>
                (string.IsNullOrWhiteSpace(Typename) || healthcondtype.Title.Value.Contains(Typename)) &&
                (!CreationDateFrom.HasValue || healthcondtype.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || healthcondtype.CreatedAt <= CreationDateTo.Value);
        }
    }
}
