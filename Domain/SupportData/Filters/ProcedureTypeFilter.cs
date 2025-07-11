using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class ProcedureTypeFilter : IEntityFilter<ProcedureType>
    {
        public string? Typename { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<ProcedureType, bool>> ToPredicate()
        {
            return proceduretype =>
                (string.IsNullOrWhiteSpace(Typename) || proceduretype.Title.Value.Contains(Typename)) &&
                (!CreationDateFrom.HasValue || proceduretype.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || proceduretype.CreatedAt <= CreationDateTo.Value);
        }
    }
}
