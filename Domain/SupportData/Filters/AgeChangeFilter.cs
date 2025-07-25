using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class AgeChangeFilter : IEntityFilter<AgeChange>
    {
        public string? Typename { get; set; }
        public string? PatientName{ get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<AgeChange, bool>> ToPredicate()
        {
            return agechange =>
                (string.IsNullOrWhiteSpace(Typename) || agechange.Type.Title.Value.Contains(Typename)) &&
                (string.IsNullOrWhiteSpace(PatientName) || agechange.PatientCard.Patient.Fullname.Value.Contains(PatientName)) &&
                (!CreationDateFrom.HasValue || agechange.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || agechange.CreatedAt <= CreationDateTo.Value);
        }
    }
}
