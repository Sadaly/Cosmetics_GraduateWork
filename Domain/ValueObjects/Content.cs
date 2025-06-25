using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects
{
    [ComplexType]
    public class Content : ValueObject
    {
        public const int MAX_LENGTH = 5000;
        public const string DEFAULT_VALUE = "Content DEFAULT_VALUE";

        public Content()
        {
            Value = DEFAULT_VALUE;
        }
        private Content(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static Content CreateDefault() { return new Content(); }
        /// <summary>
        /// Создание экземпляра <see cref="Content"/> с проверкой входящих значений
        /// </summary>
        /// <param name="stringText">Строка с текстом</param>
        /// <returns>Новый экземпляр <see cref="Content"/></returns>
        public static Result<Content> Create(string stringText)
        {
            if (string.IsNullOrWhiteSpace(stringText))
            {
                return Result.Failure<Content>(DomainErrors.Content.Empty);
            }

            if (stringText.Length > MAX_LENGTH)
            {
                return Result.Failure<Content>(DomainErrors.Content.TooLong);
            }

            return new Content(stringText);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
