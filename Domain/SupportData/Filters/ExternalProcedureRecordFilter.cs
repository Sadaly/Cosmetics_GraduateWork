using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class ExternalProcedureRecordFilter : IEntityFilter<ExternalProcedureRecord>
    {
        public string? Typename { get; set; }
        public string? PatientName { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<ExternalProcedureRecord, bool>> ToPredicate()
        {
            return externalprocedurerecord =>
                (string.IsNullOrWhiteSpace(Typename) || externalprocedurerecord.Type.Title.Value.Contains(Typename)) &&
                (string.IsNullOrWhiteSpace(PatientName) || externalprocedurerecord.PatientCard.Patient.Fullname.Value.Contains(PatientName)) &&
                (!CreationDateFrom.HasValue || externalprocedurerecord.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || externalprocedurerecord.CreatedAt <= CreationDateTo.Value);
        }
    }
}
