using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPS.Common.Collections
{
    public interface IPagedCollection<T>
    {
        int TotalRowCount { get; set; }
        ICollection<T> Data { get; set; }
    }
}
