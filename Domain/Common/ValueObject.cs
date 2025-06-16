namespace Domain.Common;

/// <summary>
/// Базовый класс для объектов-значений (Value Objects) в Domain-Driven Design.
/// Объекты-значения идентифицируются по их свойствам, а не по идентификатору.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Возвращает значения всех свойств объекта-значения.
    /// </summary>
    /// <returns>Коллекция значений свойств.</returns>
    public abstract IEnumerable<object> GetAtomicValues();

    /// <summary>
    /// Сравнивает текущий объект-значение с другим объектом-значением.
    /// </summary>
    /// <param name="other">Другой объект-значение для сравнения.</param>
    /// <returns>True, если объекты равны; иначе False.</returns>
    public bool Equals(ValueObject? other)
    {
        return other is not null && ValuesAreEqual(other);
    }

    /// <summary>
    /// Сравнивает текущий объект-значение с другим объектом.
    /// </summary>
    /// <param name="obj">Другой объект для сравнения.</param>
    /// <returns>True, если объекты равны; иначе False.</returns>
    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && ValuesAreEqual(other);
    }

    /// <summary>
    /// Вычисляет хэш-код на основе значений свойств объекта-значения.
    /// </summary>
    /// <returns>Хэш-код объекта.</returns>
    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(
                default(int),
                HashCode.Combine);
    }

    /// <summary>
    /// Проверяет, равны ли значения свойств текущего объекта и другого объекта-значения.
    /// </summary>
    /// <param name="other">Другой объект-значение для сравнения.</param>
    /// <returns>True, если значения свойств равны; иначе False.</returns>
    private bool ValuesAreEqual(ValueObject other)
    {
        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }
}