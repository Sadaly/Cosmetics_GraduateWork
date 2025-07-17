using Domain.Errors;
using Domain.ValueObjects;

namespace Application.UnitTests.TheoryData
{
    public class TestsTheoryData
    {
        public static TheoryData<string> InvalidPrimaryKeyGetTestCases = new()
        {
            { Guid.NewGuid().ToString() },
            { Guid.Empty.ToString() }
        };
        
        public static TheoryData<string> ValidNameGetTestCases = new()
        {
            { "Fullname" },
            { "llna" },
            { "Fu" },
            { "me" },
            { "" },
        };
        public static TheoryData<string> ValidOnlyOneNameGetTestCases = new()
        {
            { "Fullname1" },
            { "Fullname2" },
        };

        public static TheoryData<string> InvalidNameGetTestCases = new()
        {
            { "+++" },
            { "+" },
            { "Fe" },
            { "Fullnamee" },
        };

        public static TheoryData<string, string> InvalidNameCreationTestCases = new()
        {
            { "", DomainErrors.Username.Empty.Code },
            { new string('a', Username.MAX_LENGTH + 1), DomainErrors.Username.TooLong.Code },
            { new string('a', Username.MIN_LENGTH - 1), DomainErrors.Username.TooShort.Code }
        };

        public static TheoryData<string> ValidNameCreationTestCases = new()
        {
            { new string('a', Username.MAX_LENGTH - Username.MIN_LENGTH)},
            { new string('a', Username.MAX_LENGTH) },
            { new string('a', Username.MIN_LENGTH) }
        };

        public static TheoryData<DateTime?, DateTime?> ValidCreationDatesGetTestCases = new()
        {
            { DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1) },
            { null, DateTime.UtcNow.AddHours(1) },
            { DateTime.UtcNow.AddHours(-1), null },
            { null, null },
        };

        public static TheoryData<DateTime?, DateTime?> InvalidCreationDatesGetTestCases = new()
        {
            { DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(-1) },
            { null, DateTime.UtcNow.AddHours(-1) },
            { DateTime.UtcNow.AddHours(1), null },
        };

        public static TheoryData<int, int> ValidIndexesGetAllTestCases = new()
        {
            { 0, 1 },
            { 0, 2 },
            { 1, 2 },
        };
    }
}
