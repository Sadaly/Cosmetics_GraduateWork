namespace Persistence
{
    internal static class TableNames
    {
        internal const string User = nameof(User);
        internal const string Patient = nameof(Patient); 
        internal const string PatientCard = nameof(PatientCard);
        internal const string SkinFeature = nameof(SkinFeature);
        internal const string SkinFeatureType = nameof(SkinFeatureType);
        internal const string AgeChange = nameof(AgeChange);
        internal const string AgeChangeType = nameof(AgeChangeType);
        internal const string Doctor = nameof(Doctor);
        internal const string HealthCond = nameof(HealthCond);
        internal const string HealthCondType = nameof(HealthCondType);
        internal const string Notification = nameof(Notification);
        internal const string PatientSpecifics = nameof(PatientSpecifics);
        internal const string Procedure = nameof(Procedure);
        internal const string ProcedureType = nameof(ProcedureType);
        internal const string ReservedDate = nameof(ReservedDate);
        internal const string SkinCare = nameof(SkinCare);
        internal const string SkinCareType = nameof(SkinCareType);
        internal const string ExternalProcedureRecord = nameof(ExternalProcedureRecord);
        internal const string ExternalProcedureRecordType = nameof(ExternalProcedureRecordType);
        
        internal static class UserTable
        {
            internal const string Id = nameof(Id);
            internal const string Username = nameof(Username);
            internal const string Email = nameof(Email);
            internal const string PasswordHash = nameof(PasswordHash);
        }
    }
}
