using Domain.Enums;
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
        public static TheoryData<int, int> InvalidIndexesGetAllTestCases = new()
        {
            { -1, 1 },
            { 0, 0 },
            { -1, 0 },
        };

        public static TheoryData<string, string> ValidEntityWithTypeGuidsTestCases = new()
        {
            { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
        };
        public static TheoryData<string, string> InvalidEntityWithTypeGuidsTestCases = new()
        {
            { Guid.Empty.ToString(), Guid.Empty.ToString() },
            { Guid.NewGuid().ToString(), Guid.Empty.ToString() },
            { Guid.Empty.ToString(), Guid.NewGuid().ToString() },
        };

        public static TheoryData<string> ValidOnlyOneTitleGetTestCases = new()
        {
            { "Title1" },
            { "Title2" },
        };
        public static TheoryData<string, string> InvalidTitleCreationTestCases = new()
        {
            { new string('a', Title.MAX_LENGTH + 1), DomainErrors.Title.TooLong.Code },
        };

        public static TheoryData<string> ValidTitleCreationTestCases = new()
        {
            { new string('a', Title.MAX_LENGTH) },
            { new string('a', 0) }
        };

        public static TheoryData<string, string> InvalidEmailCreationTestCases = new()
        {
            { "", DomainErrors.Email.Empty.Code },
            { new string('a', Email.MAX_LENGTH + 1), DomainErrors.Email.TooLong.Code },
            { new string('a', Email.MIN_LENGTH - 1), DomainErrors.Email.TooShort.Code }
        };

        public static TheoryData<string> ValidEmailCreationTestCases = new()
        {
            { new string('a', Email.MAX_LENGTH - Email.MIN_LENGTH - 3) + "@" + "aa"},
            { new string('a', Email.MAX_LENGTH - 3) + "@" + "aa" },
            { new string('a', Email.MIN_LENGTH - 3) + "@" + "aa" }
        };

        public static TheoryData<string, string> InvalidPasswordCreationTestCases = new()
        {
            { "", DomainErrors.PasswordHashed.Empty.Code },
            { new string('a', PasswordHashed.MAX_LENGTH + 1), DomainErrors.PasswordHashed.TooLong.Code },
            { new string('a', PasswordHashed.MIN_LENGTH - 1), DomainErrors.PasswordHashed.TooShort.Code }
        };

        public static TheoryData<string> ValidPasswordCreationTestCases = new()
        {
            { new string('a', PasswordHashed.MAX_LENGTH - PasswordHashed.MIN_LENGTH)},
            { new string('a', PasswordHashed.MAX_LENGTH) },
            { new string('a', PasswordHashed.MIN_LENGTH) }
        };

        public static TheoryData<string, string, string, string> InvalidPatientCardUpdateTestCases = new()
        {
            { new string('a', Text.MAX_LENGTH + 1), "", "", DomainErrors.Text.TooLong },
            { "", new string('a', Text.MAX_LENGTH + 1), "", DomainErrors.Text.TooLong },
            { new string('a', Text.MAX_LENGTH + 1), new string('a', Text.MAX_LENGTH + 1), "", DomainErrors.Text.TooLong },
            { "", "", new string('1', PhoneNumber.MAX_LENGTH + 1), DomainErrors.PhoneNumber.TooLong },
            { "", "", new string('1', PhoneNumber.MIN_LENGTH - 1), DomainErrors.PhoneNumber.TooShort },
            { "", "", new string('a', PhoneNumber.MAX_LENGTH), DomainErrors.PhoneNumber.Empty },
            { "", "", new string('a', PhoneNumber.MIN_LENGTH), DomainErrors.PhoneNumber.Empty },
        };

        public static TheoryData<string, string, string> ValidPatientCardUpdateTestCases = new()
        {
            { "", "", "" },
            { new string('a', Text.MAX_LENGTH), "", "" },
            { new string('a', Text.MAX_LENGTH), new string('a', Text.MAX_LENGTH), "" },
            { new string('a', Text.MAX_LENGTH), new string('a', Text.MAX_LENGTH), new string('1', PhoneNumber.MAX_LENGTH) },

            { "", new string('a', Text.MAX_LENGTH), "" },
            { "", new string('a', Text.MAX_LENGTH), new string('1', PhoneNumber.MAX_LENGTH) },
            { "", new string('a', Text.MAX_LENGTH), new string('1', PhoneNumber.MIN_LENGTH) },
            { new string('a', Text.MAX_LENGTH), new string('a', Text.MAX_LENGTH), new string('1', PhoneNumber.MIN_LENGTH) },
        };

        public static TheoryData<ReservedDateType> ValidReservedDateTypeCreationTestCases = new()
        {
            { ReservedDateType.None },
            { ReservedDateType.TimeRestrict },
            { ReservedDateType.HolidayRestrict },
            { ReservedDateType.DayOfWeekRestrict },
        };

        public static TheoryData<string, string, string, string> ValidPatientSpecificsesUpdateTestCases = new()
        {
            { "", "", "", "" },
            { "a", "", "", "" },
            { "", "a", "", "" },
            { "", "", "a", "" },
            { "", "", "", "a" },
            { "a", "a", "a", "a" },
        };
    }
}
