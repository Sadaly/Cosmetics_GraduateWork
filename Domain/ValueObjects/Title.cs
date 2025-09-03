using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ValueObjects
{
	[ComplexType]
	public class Title : ValueObject
	{
		public const int MAX_LENGTH = 100;
		public const string DEFAULT_VALUE = "";

		private Title(string value)
		{
			Value = value;
		}

		public string Value { get; set; }

		public static Title CreateDefault() { return new Title(); }

		/// <summary>
		/// Создание экземпляра <see cref="Title"/>  с проверкой входящих значений
		/// </summary>
		/// <param name="title">Строка с названием</param>
		/// <returns>Новый экземпляр <see cref="Title"/></returns>
		public static Result<Title> Create(string? title)
		{
			if (string.IsNullOrWhiteSpace(title))
				return CreateDefault();

			if (title.Length > MAX_LENGTH)
				return Result.Failure<Title>(DomainErrors.Title.TooLong);

			return new Title(title);
		}
		public override IEnumerable<object> GetAtomicValues()
		{
			yield return Value;
		}
		internal Title() { Value = DEFAULT_VALUE; }
	}
}
