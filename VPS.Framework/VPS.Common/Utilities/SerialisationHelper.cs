using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VPS.Common.Utilities
{
    public class SerialisationHelper
    {
        private static readonly Dictionary<Type, XmlSerializer> _serialisers = new Dictionary<Type, XmlSerializer>();

        public static string SerialiseToXmlString<T>(T objectToSerialise)
        {
            var serialiser = CreateSerialiser<T>();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var writer = new StringWriter())
            {
                serialiser.Serialize(writer, objectToSerialise, ns);
                
                writer.Flush();
	            return writer.ToString();
            }
        }

	    private static XmlSerializer CreateSerialiser<T>()
	    {
		    var type = typeof(T);
		    if (!_serialisers.ContainsKey(type))
		    {
			    _serialisers[type] = new XmlSerializer(type);
		    }
		    return _serialisers[type];
	    }

	    public static T DeserialiseFromXmlString<T>(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString)) return default(T);

            T returnType;

            var serialiser = CreateSerialiser<T>();

            using (var textReader = new StringReader(xmlString))
            {
                returnType = (T)serialiser.Deserialize(textReader);
            }

            return returnType; 
        }
    }
}
