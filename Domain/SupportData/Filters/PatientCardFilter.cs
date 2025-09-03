using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
	public class PatientCardFilter : IEntityFilter<PatientCard>
	{
		public string? PatientName { get; set; }
		public string? Adress { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Complaints { get; set; }
		public byte? FromAge { get; set; }
		public byte? ToAge { get; set; }
		public DateTime? CreationDateFrom { get; set; }
		public DateTime? CreationDateTo { get; set; }

		public Expression<Func<PatientCard, bool>> ToPredicate()
		{
			return patientcard =>
				(string.IsNullOrWhiteSpace(PatientName) || patientcard.Patient.Fullname.Value.Contains(PatientName)) &&
				(string.IsNullOrWhiteSpace(Adress) || patientcard.Adress.Value.Contains(Adress)) &&
				(string.IsNullOrWhiteSpace(PhoneNumber) || patientcard.PhoneNumber.Value.Contains(PhoneNumber)) &&
				(string.IsNullOrWhiteSpace(Complaints) || patientcard.Complaints.Value.Contains(Complaints)) &&
				(!FromAge.HasValue || FromAge < patientcard.Age) &&
				(!ToAge.HasValue || patientcard.Age < ToAge) &&
				(!CreationDateFrom.HasValue || patientcard.CreatedAt >= CreationDateFrom.Value) &&
				(!CreationDateTo.HasValue || patientcard.CreatedAt <= CreationDateTo.Value);
		}
	}
}
