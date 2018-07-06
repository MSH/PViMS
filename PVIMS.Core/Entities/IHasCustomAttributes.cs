using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	internal interface IHasCustomAttributes
	{
		[Column(TypeName = "xml")]
		string CustomAttributesXmlSerialised { get; set; }
	}
}