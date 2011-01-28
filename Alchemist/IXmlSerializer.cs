using System.IO;

namespace Alchemist
{
	public interface IXmlSerializer
	{
		void Serialize( Stream s, object o );
		object Deserialize( Stream s );
	}
}