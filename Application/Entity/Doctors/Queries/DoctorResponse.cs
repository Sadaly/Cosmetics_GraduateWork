using Domain.Entity;

namespace Application.Entity.Doctors.Queries;

public sealed record DoctorResponse(Guid Id, string Name)
{
    internal DoctorResponse(Doctor ageChange) : this(ageChange.Id, ageChange.Fullname.Value) 
    { }
}
