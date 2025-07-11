using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class ExternalProcedureRecordTypeFilter : IEntityFilter<ExternalProcedureRecordType>
    {
        public string? Typename { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<ExternalProcedureRecordType, bool>> ToPredicate()
        {
            return externalprocedurerecordtype =>
                (string.IsNullOrWhiteSpace(Typename) || externalprocedurerecordtype.Title.Value.Contains(Typename)) &&
                (!CreationDateFrom.HasValue || externalprocedurerecordtype.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || externalprocedurerecordtype.CreatedAt <= CreationDateTo.Value);
        }
    }
}
