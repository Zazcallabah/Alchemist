using System.Configuration;
using System.IO;

namespace Alchemist
{
	class Program
	{
		static void Main()
		{
			var filename = ConfigurationManager.AppSettings["filename"];
			var rs = FetchRuleSet( filename );
			var controller = new AlchemyController( rs );
			var communicator = new Communicator();
			var chemist = new Chemist( controller, communicator );

			chemist.Cook();
		}

		static RuleSet FetchRuleSet( string filename )
		{
			var persistance = new XmlPersister( new RuleSetXmlSerializer(), new StreamFactory( filename ) );
			var rs = !File.Exists( filename ) ? new RuleSet() : persistance.RecreateRuleSet();
			persistance.RegisterRuleSet( rs );
			return rs;
		}
	}
}
