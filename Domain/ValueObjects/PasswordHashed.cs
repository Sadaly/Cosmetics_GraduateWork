using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Domain.ValueObjects
{
    [ComplexType]
    public class PasswordHashed : ValueObject
    {
        public const int MAX_LENGTH = 100;
        public const int MIN_LENGTH = 8;
        public const string DEFAULT_VALUE = "PasswordHashed DEFAULT_VALUE";

        public string Value { get; set; }

        private PasswordHashed(string value)
        {
            Value = value;
        }

        public static Result<PasswordHashed> Create(string value)
        {
            // Обрабатываем логически ошибки
            if (string.IsNullOrEmpty(value)) return Result.Failure<PasswordHashed>(DomainErrors.PasswordHashed.Empty);
            if (value.Length > MAX_LENGTH) return Result.Failure<PasswordHashed>(DomainErrors.PasswordHashed.TooLong);
            if (value.Length < MIN_LENGTH) return Result.Failure<PasswordHashed>(DomainErrors.PasswordHashed.TooShort);

            var hash = PasswordHashed.HashPassword(value);

            return new PasswordHashed(hash);
        }
        public static Result<PasswordHashed> CreateOrDefault(string? value)
        {
            return value == null
                ? new PasswordHashed()
                : Create(value);
        }
        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        /// <summary>
        /// Метод хеширования пароля
        /// </summary>
        /// <param name="password">Пароль пользователя</param>
        /// <returns></returns>
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Преобразуем байты в hex-строку
                }
                return builder.ToString();
            }
        }


        /// <summary>
        /// Это просто заглушка для компилятора. Т.к. для EF нужен конструктор без параметров,
        /// тогда Value объекты остаются null и этот конструктор решает эту проблему
        /// </summary>
        internal PasswordHashed() { Value = DEFAULT_VALUE; }
    }
}
