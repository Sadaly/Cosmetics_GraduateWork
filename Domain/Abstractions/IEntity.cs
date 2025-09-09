using Domain.Shared;

namespace Domain.Abstractions
{
	internal interface IEntity
	{
		/// <summary>
		/// Метод отмечает сущность для удаления. Это позволит восстанавливать данные на уровне бд
		/// </summary>
		/// <returns></returns>
		public Result SoftDelete();
	}
}
