namespace Application.Abstractions
{
    public interface IProcedureScheduleService
    {
        public Task<bool> IsDateReserved(DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken);
    }
}
