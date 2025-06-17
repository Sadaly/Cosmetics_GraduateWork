using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects
{
    [ComplexType]
    public class Username : ValueObject
    {
        public const int MAX_LENGTH = 30;
        public const int MIN_LENGTH = 3;

        public string Value { get; set; }

        private Username(string value)
        {
            Value = value;
        }

        public static Result<Username> Create (string value)
        {
            // Обрабатываем логически ошибки
            if (string.IsNullOrEmpty(value)) return Result.Failure<Username>(DomainErrors.Username.Empty);
            if (value.Length > MAX_LENGTH) return Result.Failure<Username>(DomainErrors.Username.TooLong);
            if (value.Length < MIN_LENGTH) return Result.Failure<Username>(DomainErrors.Username.TooShort);

            return new Username(value);
        }
        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }


        /// <summary>
        /// Это просто заглушка для компилятора. Т.к. для EF нужен конструктор без параметров,
        /// тогда Value объекты остаются null и этот конструктор решает эту проблему
        /// </summary>
        internal Username() { Value = ""; }
    }
}
