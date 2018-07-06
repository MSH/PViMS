using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPS.CustomAttributes
{
    public interface ISelectionDataRepository
    {
        /// <summary>
        /// Retrieves the selection data for the specified attribute key.
        /// </summary>
        /// <param name="attributeKey">The attribute key.</param>
        /// <returns></returns>
        ICollection<SelectionDataItem> RetrieveSelectionDataForAttribute(string attributeKey);

        /// <summary>
        /// Retrieves all selection data.
        /// </summary>
        /// <returns></returns>
        ICollection<SelectionDataItem> RetrieveAllSelectionData();
    }
}
