using Domain.Common;
using Domain.Shared;
using System.Linq.Expressions;

namespace Domain.Abstractions
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Добавление нового экземпляра. При этом передаваемый экземпляр изменяет свой Id, если Id есть вообще как поле.
        /// </summary>
        /// <param name="entity">Ссылка на entity.</param>
        Task<Result<T>> AddAsync(Result<T> entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновляет значения на основе переданного экземпляра.
        /// </summary>
        /// <param name="entity">Измененный экземпляр.</param>
        Task<Result<T>> UpdateAsync(Result<T> entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаление экземпляра.
        /// </summary>
        /// <param name="entity">Ссылка на entity.</param>
        Task<Result<T>> RemoveAsync(Result<T> entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает все объекты.
        /// </summary>
        /// <returns>Возвращает лист, предполагаю что это поменяется когда я возьмусь за реализацию.</returns>
        Task<Result<List<T>>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает часть объектов.
        /// </summary>
        /// <param name="startIndex">Начальный индекс. 1-ый элемент под индексом 0.</param>
        /// <param name="count">Количество взятых значений.</param>
        /// <returns>Список из объектов. Если <paramref name="startIndex"/> или <paramref name="count"/> выходят за рамки БД, ошибки не будет, вернется лишь часть данных, которая находится в рамках списка записей.</returns>
        Task<Result<List<T>>> GetAllAsync(int startIndex, int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает все объекты, которые соответствуют предикату.
        /// </summary>
        /// <param name="predicate">Условия для списка.</param>
        /// <returns>Возвращает список, все объекты которого удовлетворяют условию <paramref name="predicate"/>.</returns>
        Task<Result<List<T>>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод возвращающий часть данных из БД по указанному предикату (сначала выполняется условие, а затем берутся элементы).
        /// </summary>
        /// <param name="predicate">Условия для списка.</param>
        /// <param name="startIndex">Начальный индекс. 1-ый элемент под индексом 0.</param>
        /// <param name="count">Количество взятых значений.</param>
        /// <returns>Возвращает часть списка элементы которого удовлетворяют условию <paramref name="predicate"/>. Если <paramref name="startIndex"/> или <paramref name="count"/> выходят за рамки БД, ошибки не будет, вернется лишь часть данных, которая находится в рамках списка записей.</returns>
        public Task<Result<List<T>>> GetAllAsync(int startIndex, int count, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получение сущности типа репозитория по предикату
        /// </summary>
        /// <param name="predicate">Условия для получения сущности.</param>
        /// <returns>Если объект не найден, то будет возвращена ошибка.</returns>
        public Task<Result<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// Получение сущности типа репозитория по Id с учетом ошибки
        /// </summary>
        /// <returns>Если объект не найден, то будет возвращена ошибка.</returns>
        public Task<Result<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
