using Domain.Repositories;
using Application.Abstractions;

namespace Infrastructure.Services
{
    public class ProcedureScheduleService(IReservedDateRepository reservedDateRepository) : IProcedureScheduleService
    {
        /// <summary>
        /// Метод для проверки того, может ли быть назначена запись на определенную дату и время, если отрезок времени записи пересекается
        /// с любым из отрезков дат и времен зарезервированных записей, то можно сказать, что часть времени назначаемой записи зарезервирована, 
        /// и поэтому мы должны уведомить об этом пользователя
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> IsDateReserved(DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken)
        {
            //Логика проверки следующая, rd - дата и/или время в которую нельзя ничего записать,
            //если rd.Начальная дата попадает в промежуток записи, то это значит, что они пересекаются,
            //то же самое и с rd.Конечная дата, если попадает в промежуток записи, то пересекаются,
            //если отрезок времени записи с координатами (запись.Начальная дата, запись.Конечная дата) находится в отрезке
            //времени rd с координатами (rd.Начальная дата, rd.Конечная дата), то мы тоже считаем, что время записи попадает
            //в зарезервированное время

            //Резервированные даты бывают циклическими и единовременными ниже представлены некоторые из них
            var reserevedDates = await reservedDateRepository.GetAllAsync(cancellationToken);

            //единовременная (стоит переименовать в DateRestrict и создать отдельный HolidayRestr, но возможно и не стоит,
            //т.к. ИП само решает в какие дни работать), представляет единовременно резервированную дату
            var rd = await reservedDateRepository.GetByPredicateAsync(
                x =>
                x.Type == Domain.Enums.ReservedDateType.HolidayRestrict
                && ((x.StartDate < EndDate && x.StartDate > StartDate)
                || (x.EndDate < EndDate && x.EndDate > StartDate)
                || (x.StartDate <= StartDate && x.EndDate >= EndDate)),
                cancellationToken);

            if (rd.IsSuccess) return true;

            //циклическая, представляет собой ежедневное ограничение по времени, например: рабочие часы.
            rd = await reservedDateRepository.GetByPredicateAsync(
                x =>
                x.Type == Domain.Enums.ReservedDateType.TimeRestrict
                && ((x.StartDate.TimeOfDay < EndDate.TimeOfDay && x.StartDate.TimeOfDay > StartDate.TimeOfDay)
                || (x.EndDate.TimeOfDay < EndDate.TimeOfDay && x.EndDate.TimeOfDay > StartDate.TimeOfDay)
                || (x.StartDate.TimeOfDay <= StartDate.TimeOfDay && x.EndDate.TimeOfDay >= EndDate.TimeOfDay)),
                cancellationToken);

            if (rd.IsSuccess) return true;

            //циклическая, представляет собой еженедельные ограничение по времени, например: рабочие дни.
            rd = await reservedDateRepository.GetByPredicateAsync(
                x =>
                x.Type == Domain.Enums.ReservedDateType.DayOfWeekRestrict
                && ((x.StartDate.DayOfWeek < EndDate.DayOfWeek && x.StartDate.DayOfWeek > StartDate.DayOfWeek)
                || (x.EndDate.DayOfWeek < EndDate.DayOfWeek && x.EndDate.DayOfWeek > StartDate.DayOfWeek)
                || (x.StartDate.DayOfWeek <= StartDate.DayOfWeek && x.EndDate.DayOfWeek >= EndDate.DayOfWeek)),
                cancellationToken);

            if (rd.IsSuccess) return true;

            return false;
        }
    }
}