using Domain.Repositories;
using Application.Abstractions;

namespace Infrastructure.Services
{
    public class ProcedureScheduleService(IReservedDateRepository reservedDateRepository) : IProcedureScheduleService
    {
        public async Task<bool> IsDateReserved(DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken)
        {
            var reserevedDates = await reservedDateRepository.GetAllAsync(cancellationToken);

            var rd = await reservedDateRepository.GetByPredicateAsync(
                x =>
                x.Type == Domain.Enums.ReservedDateType.HolidayRestrict
                && ((x.StartDate < EndDate && x.EndDate >= EndDate)
                || (x.StartDate >= StartDate && x.EndDate < StartDate)
                || (x.StartDate > StartDate && x.EndDate < EndDate)),
                cancellationToken);

            if (rd.IsSuccess) return true;

            rd = await reservedDateRepository.GetByPredicateAsync(
                x =>
                x.Type == Domain.Enums.ReservedDateType.TimeRestrict
                && ((x.StartDate.TimeOfDay < EndDate.TimeOfDay && x.EndDate.TimeOfDay >= EndDate.TimeOfDay)
                || (x.StartDate.TimeOfDay >= StartDate.TimeOfDay && x.EndDate.TimeOfDay < StartDate.TimeOfDay)
                || (x.StartDate.TimeOfDay > StartDate.TimeOfDay && x.EndDate.TimeOfDay < EndDate.TimeOfDay)),
                cancellationToken);

            if (rd.IsSuccess) return true; 
            
            rd = await reservedDateRepository.GetByPredicateAsync(
                x =>
                x.Type == Domain.Enums.ReservedDateType.DayOfWeekRestrict
                && ((x.StartDate.DayOfWeek < EndDate.DayOfWeek && x.EndDate.DayOfWeek >= EndDate.DayOfWeek)
                || (x.StartDate.DayOfWeek >= StartDate.DayOfWeek && x.EndDate.DayOfWeek < StartDate.DayOfWeek)
                || (x.StartDate.DayOfWeek > StartDate.DayOfWeek && x.EndDate.DayOfWeek < EndDate.DayOfWeek)),
                cancellationToken);

            if (rd.IsSuccess) return true;

            return false;
        }
    }
}
