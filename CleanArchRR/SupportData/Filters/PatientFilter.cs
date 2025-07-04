using Domain.Entity;
using System.Linq.Expressions;
using WebApi.Abstractions;

namespace WebApi.SupportData.Filters
{
    public class PatientFilter : IEntityFilter<Patient>
    {
        public string? Fullname { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<Patient, bool>> ToPredicate()
        {
            return patient =>
                (string.IsNullOrWhiteSpace(Fullname) || patient.Fullname.Value.Contains(Fullname)) &&
                (!CreationDateFrom.HasValue || patient.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || patient.CreatedAt <= CreationDateTo.Value);
        }
    }
}
