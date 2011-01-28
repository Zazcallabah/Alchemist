using System;
using System.ComponentModel;

namespace Alchemist
{
	public class XmlPersister
	{
		readonly IXmlSerializer _serializer;
		readonly IStreamFactory _streamfactory;

		public XmlPersister( IXmlSerializer serializer, IStreamFactory streamfactory )
		{
			_serializer = serializer;
			_streamfactory = streamfactory;
		}

		public void RegisterRuleSet( RuleSet rs )
		{
			rs.PropertyChanged += RuleSetPropertyChanged;
		}

		void RuleSetPropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			try
			{
				using( var stream = _streamfactory.CreateSerializingStream() )
					_serializer.Serialize( stream, sender );
			}
			catch( System.IO.IOException )
			{
				Console.WriteLine( "Warning: could not serialize data." );
			}
		}

		public RuleSet RecreateRuleSet()
		{
			using( var stream = _streamfactory.CreateDeserializingStream() )
				return (RuleSet) _serializer.Deserialize( stream );
		}
	}
}