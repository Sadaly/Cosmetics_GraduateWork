using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class SkinCareTypeFilter : IEntityFilter<SkinCareType>
    {
        public string? Typename { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<SkinCareType, bool>> ToPredicate()
        {
            return skincaretype =>
                (string.IsNullOrWhiteSpace(Typename) || skincaretype.Title.Value.Contains(Typename)) &&
                (!CreationDateFrom.HasValue || skincaretype.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || skincaretype.CreatedAt <= CreationDateTo.Value);
        }
    }
}
