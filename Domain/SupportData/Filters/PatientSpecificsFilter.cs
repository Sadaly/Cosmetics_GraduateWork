using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
	public class PatientSpecificsFilter : IEntityFilter<PatientSpecifics>
	{
		public string? PatientName { get; set; }
		public string? Diet { get; set; }
		public string? Sleep { get; set; }
		public string? Sport { get; set; }
		public string? WorkEnviroment { get; set; }
		public DateTime? CreationDateFrom { get; set; }
		public DateTime? CreationDateTo { get; set; }

		public Expression<Func<PatientSpecifics, bool>> ToPredicate()
		{
			return patientspecifics =>
				(string.IsNullOrWhiteSpace(Diet) || patientspecifics.Diet.Contains(Diet)) &&
				(string.IsNullOrWhiteSpace(Sleep) || patientspecifics.Sleep.Contains(Sleep)) &&
				(string.IsNullOrWhiteSpace(Sport) || patientspecifics.Sport.Contains(Sport)) &&
				(string.IsNullOrWhiteSpace(WorkEnviroment) || patientspecifics.WorkEnviroment.Contains(WorkEnviroment)) &&
				(string.IsNullOrWhiteSpace(PatientName) || patientspecifics.PatientCard.Patient.Fullname.Value.Contains(PatientName)) &&
				(!CreationDateFrom.HasValue || patientspecifics.CreatedAt >= CreationDateFrom.Value) &&
				(!CreationDateTo.HasValue || patientspecifics.CreatedAt <= CreationDateTo.Value);
		}
	}
}
