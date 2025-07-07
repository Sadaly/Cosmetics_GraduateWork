using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class ReservedDate : BaseEntity
    {
        private ReservedDate(Guid id) : base(id) { }
        private ReservedDate(Guid id, DateTime startDate, DateTime endDate) : base(id) {
            StartDate = startDate;
            EndDate = endDate;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [NotMapped]
        public DayOfWeek StartDateOfWeek => StartDate.DayOfWeek;
        [NotMapped]
        public DayOfWeek EndDateOfWeek => EndDate.DayOfWeek;
        [NotMapped]
        public int StartDateHour => StartDate.Hour;
        [NotMapped]
        public int EndDateHour => EndDate.Hour;

    }
}
