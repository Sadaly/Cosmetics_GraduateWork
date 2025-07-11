using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class SkinCareFilter : IEntityFilter<SkinCare>
    {
        public string? Typename { get; set; }
        public string? PatienName{ get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<SkinCare, bool>> ToPredicate()
        {
            return skincare =>
                (string.IsNullOrWhiteSpace(Typename) || skincare.Type.Title.Value.Contains(Typename)) &&
                (string.IsNullOrWhiteSpace(PatienName) || skincare.PatientCard.Patient.Fullname.Value.Contains(PatienName)) &&
                (!CreationDateFrom.HasValue || skincare.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || skincare.CreatedAt <= CreationDateTo.Value);
        }
    }
}
