using Domain.Abstractions;
using Domain.Entity;
using System.Linq.Expressions;

namespace Domain.SupportData.Filters
{
    public class NotificationFilter : IEntityFilter<Notification>
    {
        public string? Message { get; set; }
        public bool? IsSent { get; set; }
        public string? Phone { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }

        public Expression<Func<Notification, bool>> ToPredicate()
        {
            return notification =>
                (string.IsNullOrWhiteSpace(Message) || notification.Message.Value.Contains(Message)) &&
                (string.IsNullOrWhiteSpace(Phone) || notification.PhoneNumber.Value.Contains(Phone)) &&
                (string.IsNullOrWhiteSpace(PatientName) || notification.Procedure.PatientCard.Patient.Fullname.Value.Contains(PatientName)) &&
                (string.IsNullOrEmpty(DoctorName)
                    || (notification.Procedure.Doctor != null && notification.Procedure.Doctor.Fullname.Value.Contains(DoctorName))
                    || (string.IsNullOrWhiteSpace(DoctorName) && notification.Procedure.Doctor != null)) &&
                (!IsSent.HasValue || notification.IsSent == IsSent) &&
                (!CreationDateFrom.HasValue || notification.CreatedAt >= CreationDateFrom.Value) &&
                (!CreationDateTo.HasValue || notification.CreatedAt <= CreationDateTo.Value);
        }
    }
}
