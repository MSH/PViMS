using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VPS.Common.Collections
{
	// TODO: This should implement ICollection, not compose it.
	public class PagedCollection<T> : IPagedCollection<T>
	{
		public PagedCollection()
		{
			Data = new Collection<T>();
		}

		public PagedCollection(IEnumerable<T> data)
		{
			Data = new Collection<T>(new List<T>(data));
		}

		public int TotalRowCount { get; set; }
		public ICollection<T> Data { get; set; }
	}
}