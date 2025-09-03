namespace Application.Abstractions
{
	public interface IProcedureScheduleService
	{
		Task<bool> IsDateReserved(DateTime StartDate, DateTime EndDate, CancellationToken cancellationToken);
	}
}
