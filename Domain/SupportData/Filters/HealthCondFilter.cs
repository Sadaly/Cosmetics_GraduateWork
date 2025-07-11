using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class HealthCondFilter : IEntityFilter<HealthCond>
    {
        public string? Typename { get; set; }
        public string? PatienName{ get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<HealthCond, bool>> ToPredicate()
        {
            return healthcond =>
                (string.IsNullOrWhiteSpace(Typename) || healthcond.Type.Title.Value.Contains(Typename)) &&
                (string.IsNullOrWhiteSpace(PatienName) || healthcond.PatientCard.Patient.Fullname.Value.Contains(PatienName)) &&
                (!CreationDateFrom.HasValue || healthcond.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || healthcond.CreatedAt <= CreationDateTo.Value);
        }
    }
}
