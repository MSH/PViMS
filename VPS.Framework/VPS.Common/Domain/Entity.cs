namespace VPS.Common.Domain
{
	public class Entity<T> : EqualityAndHashCodeProvider<T> where T : struct
	{
	}
}