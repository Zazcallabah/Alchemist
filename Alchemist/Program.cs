using System;
using System.Configuration;
using System.IO;

namespace Alchemist
{
	class Program
	{
		static void Main()
		{
			var filename = ConfigurationManager.AppSettings["filename"];
			var serializationTime = Int32.Parse( ConfigurationManager.AppSettings["serializationTime"] );


			var rs = FetchRuleSet( filename, serializationTime );
			var controller = new AlchemyController( rs );
			var communicator = new Communicator();
			var chemist = new Chemist( controller, communicator );

			chemist.Cook();
		}

		static RuleSet FetchRuleSet( string filename, int serializationTime )
		{
			var persistance = new XmlPersister( new RuleSetXmlSerializer(), new StreamFactory( filename ), serializationTime );
			var rs = !File.Exists( filename ) ? new RuleSet() : persistance.RecreateRuleSet();
			persistance.RegisterRuleSet( rs );
			return rs;
		}
	}
}
