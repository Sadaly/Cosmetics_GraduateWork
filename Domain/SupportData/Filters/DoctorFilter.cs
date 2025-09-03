using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
	public class DoctorFilter : IEntityFilter<Doctor>
	{
		public string? Name { get; set; }
		public DateTime? CreationDateFrom { get; set; }
		public DateTime? CreationDateTo { get; set; }

		public Expression<Func<Doctor, bool>> ToPredicate()
		{
			return doctor =>
				(string.IsNullOrWhiteSpace(Name) || doctor.Fullname.Value.Contains(Name)) &&
				(!CreationDateFrom.HasValue || doctor.CreatedAt >= CreationDateFrom.Value) &&
				(!CreationDateTo.HasValue || doctor.CreatedAt <= CreationDateTo.Value);
		}
	}
}
