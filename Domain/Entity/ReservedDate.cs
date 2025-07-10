using Domain.Common;
using Domain.Enums;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class ReservedDate : BaseEntity
    {
        private ReservedDate(Guid id) : base(id) { }
        private ReservedDate(Guid id, DateTime startDate, DateTime endDate, ReservedDateType type) : base(id) {
            StartDate = startDate;
            EndDate = endDate;
            Type = type;
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
        public ReservedDateType Type { get; set; }
        public static Result<ReservedDate> Create(DateTime StartDate, DateTime EndDate, ReservedDateType Type)
        {
            return new ReservedDate(Guid.NewGuid(), StartDate, EndDate, Type);
        }
    }
}
