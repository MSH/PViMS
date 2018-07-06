using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.CustomAttributes
{
    public interface IExtendable
    {
        CustomAttributeSet CustomAttributes { get; }
        string CustomAttributesXmlSerialised { get; }
        void SetAttributeValue<T>(string attributeKey, T attributeValue, string updatedByUser);
        void ValidateAndSetAttributeValue<T>(CustomAttributeConfiguration attributeConfig, T attributeValue, string updatedByUser);
        object GetAttributeValue(string attributeKey);
        DateTime GetUpdatedDate(string attributeKey);
        string GetUpdatedByUser(string attributeKey);
    }
}
