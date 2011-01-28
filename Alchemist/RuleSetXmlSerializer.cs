using System.Xml.Serialization;

namespace Alchemist
{
	public class RuleSetXmlSerializer : XmlSerializer, IXmlSerializer
	{
		public RuleSetXmlSerializer()
			: base( typeof( RuleSet ) ) { }
	}
}