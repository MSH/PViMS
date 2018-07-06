using System.Dynamic;
using VPS.Common.Domain;

namespace VPS.Common.Repositories
{
	public interface IFactory<TEntity> where TEntity : Entity<long>
	{
		TEntity Create();
	}
}