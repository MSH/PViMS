using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.CustomAttributes
{
    public interface ITypeExtensionHandler
    {
        /// <summary>
        /// Returns a list of unpopulated Custom Attributes for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<CustomAttributeDetail> BuildModelExtension<T>() where T : IExtendable;

        /// <summary>
        /// Returns list of Custom Attributes for the specified extendable object with the values prepopulated
        /// </summary>
        /// <param name="extendableObject">The extendable object.</param>
        /// <returns></returns>
        List<CustomAttributeDetail> BuildModelExtension(IExtendable extendableObject);

        /// <summary>
        /// Refreshes the reference data for the selection type attributes in the specified collection of attributes.
        /// </summary>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <returns></returns>
        List<CustomAttributeDetail> RefreshReferenceData(List<CustomAttributeDetail> customAttributes);

        /// <summary>
        /// Updates the extendable object with values from the custom attribute collection.
        /// </summary>
        /// <param name="extendableToUpdate">The extendable to update.</param>
        /// <param name="customAttributeDetails">The custom attribute details.</param>
        /// <returns>The updated Extendable object</returns>
        /// <exception cref="CustomAttributeException">Unknown AttributeType for AttributeKey: {0}</exception>
        IExtendable UpdateExtendable(IExtendable extendableToUpdate, IEnumerable<CustomAttributeDetail> customAttributeDetails, string updatedByUser);

        /// <summary>
        /// Caches all selection data items.
        /// </summary>
        void CacheSelectionDataItems();
    }
}
