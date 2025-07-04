using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects
{
    [ComplexType]
    public class Email : ValueObject
    {
        public const int MAX_LENGTH = 50;
        public const int MIN_LENGTH = 5;
        public const int FIRST_PART_MIN_LENGTH = 1;
        public const int SECOND_PART_MIN_LENGTH = 1;
        public const string DEFAULT_VALUE = "";

        public string Value { get; set; }

        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string value)
        {
            // Обрабатываем логически ошибки
            if (string.IsNullOrWhiteSpace(value)) return Result.Failure<Email>(DomainErrors.Email.Empty);
            if (value.Length > MAX_LENGTH) return Result.Failure<Email>(DomainErrors.Email.TooLong);
            if (value.Length < MIN_LENGTH) return Result.Failure<Email>(DomainErrors.Email.TooShort);

            if (!IsValidFormat(value)) return Result.Failure<Email>(DomainErrors.Email.InvalidFormat);

            return new Email(value);
        }

        /// <summary>
        /// Проверка формата и размера левых и правых частей по разделителю '@'
        /// </summary>
        /// <param name="email">Почта</param>
        /// <returns></returns>
        public static bool IsValidFormat (string email)
        {
            var split = email.Split('@');
            if (split.Length != 2) return false;
            if (split[0].Length < FIRST_PART_MIN_LENGTH
                || split[1].Length < SECOND_PART_MIN_LENGTH)
                return false;
            return true;
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }


        /// <summary>
        /// Это просто заглушка для компилятора. Т.к. для EF нужен конструктор без параметров,
        /// тогда Value объекты остаются null и этот конструктор решает эту проблему
        /// </summary>
        internal Email() { Value = DEFAULT_VALUE; }
    }
}
