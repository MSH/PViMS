using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace VPS.EF
{
	public abstract class NonPluralDbContextBase : DbContext
	{
		protected NonPluralDbContextBase()
		{
		}

		protected NonPluralDbContextBase(string nameOrConnectionString) : base(nameOrConnectionString)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
		}
	}
}