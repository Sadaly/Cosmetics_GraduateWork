using Domain.Shared;

namespace Domain.Abstractions
{
	internal interface IEntity
	{
		public Result SoftDelete();
	}
}
