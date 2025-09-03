using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects
{
	[ComplexType]
	public class Text : ValueObject
	{
		public const int MAX_LENGTH = 5000;
		public const string DEFAULT_VALUE = "";

		private Text(string value)
		{
			Value = value;
		}

		public string Value { get; private set; }

		public static Text CreateDefault() { return new Text(); }
		/// <summary>
		/// Создание экземпляра <see cref="Text"/> с проверкой входящих значений
		/// </summary>
		/// <param name="stringText">Строка с текстом</param>
		/// <returns>Новый экземпляр <see cref="Text"/></returns>
		public static Result<Text> Create(string? stringText)
		{
			if (string.IsNullOrWhiteSpace(stringText))
				return CreateDefault();

			if (stringText.Length > MAX_LENGTH)
				return Result.Failure<Text>(DomainErrors.Text.TooLong);


			return new Text(stringText);
		}

		public override IEnumerable<object> GetAtomicValues()
		{
			yield return Value;
		}

		internal Text() { Value = DEFAULT_VALUE; }
	}
}
