using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Domain.Common;
using Domain.Errors;
using Domain.Shared;

namespace Domain.ValueObjects
{
    [ComplexType]
    public class PhoneNumber : ValueObject
    {
        public const int MAX_LENGTH = 15;
        public const int MIN_LENGTH = 8;
        public const string DEFAULT_VALUE = "";

        private PhoneNumber(string phoneNumber)
        {
            Value = phoneNumber;
        }

        public string Value { get; private set; }

        public static PhoneNumber CreateDefault() { return new PhoneNumber(); }
        /// <summary>
        /// Создание экземпляра <see cref="PhoneNumber"/> с проверкой входящих значений
        /// </summary>
        /// <param name="phoneNumber">Строка с номером телефона</param>
        /// <returns>Новый экземпляр <see cref="PhoneNumber"/></returns>
        public static Result<PhoneNumber> Create(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return CreateDefault();

            var cleanedNumber = Regex.Replace(phoneNumber, @"[^0-9+]", "");

            if (string.IsNullOrWhiteSpace(cleanedNumber))
                return Result.Failure<PhoneNumber>(DomainErrors.PhoneNumber.Empty);


            if (cleanedNumber.Length > MAX_LENGTH)
                return Result.Failure<PhoneNumber>(DomainErrors.PhoneNumber.TooLong);


            if (cleanedNumber.Length < MIN_LENGTH)
                return Result.Failure<PhoneNumber>(DomainErrors.PhoneNumber.TooShort);

            return new PhoneNumber(cleanedNumber);
        }
        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
        internal PhoneNumber() { Value = DEFAULT_VALUE; }
    }
}
