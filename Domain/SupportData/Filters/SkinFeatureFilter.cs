using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class SkinFeatureFilter : IEntityFilter<SkinFeature>
    {
        public string? Typename { get; set; }
        public string? PatientName{ get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<SkinFeature, bool>> ToPredicate()
        {
            return skinfeature =>
                (string.IsNullOrWhiteSpace(Typename) || skinfeature.Type.Title.Value.Contains(Typename)) &&
                (string.IsNullOrWhiteSpace(PatientName) || skinfeature.PatientCard.Patient.Fullname.Value.Contains(PatientName)) &&
                (!CreationDateFrom.HasValue || skinfeature.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || skinfeature.CreatedAt <= CreationDateTo.Value);
        }
    }
}
