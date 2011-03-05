using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;

namespace Alchemist
{
	public class XmlPersister
	{
		readonly IXmlSerializer _serializer;
		readonly IStreamFactory _streamfactory;
		readonly int _milliSecondsBetweenSerialization;
		Thread _currentWork;
		object _currentTarget;

		public XmlPersister( IXmlSerializer serializer, IStreamFactory streamfactory, int milliSecondsBetweenSerialization )
		{
			_serializer = serializer;
			_streamfactory = streamfactory;
			_milliSecondsBetweenSerialization = milliSecondsBetweenSerialization;
		}

		public void RegisterRuleSet( RuleSet rs )
		{
			rs.PropertyChanged += RuleSetPropertyChanged;
		}

		void PerformSerializationWork( object target )
		{
			try
			{
				using( var stream = _streamfactory.CreateSerializingStream() )
					_serializer.Serialize( stream, target );
			}
			catch( System.IO.IOException )
			{
				Console.WriteLine( "Warning: could not serialize data." );
			}
		}

		void SerializationWorker()
		{
			try
			{
				Thread.Sleep( _milliSecondsBetweenSerialization );
				RuleSet current = ( (RuleSet) _currentTarget ).DeepClone();
				PerformSerializationWork( current );
			}
			catch( Exception )
			{
			}
		}

		public void JoinCurrentSerialization()
		{
			if( _currentWork != null )
				_currentWork.Join();
		}

		void RuleSetPropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			_currentTarget = sender;
			if( _currentWork == null || !_currentWork.IsAlive )
			{
				_currentWork = new Thread( SerializationWorker );
				_currentWork.Start();
			}
		}

		public RuleSet RecreateRuleSet()
		{
			using( var stream = _streamfactory.CreateDeserializingStream() )
				return (RuleSet) _serializer.Deserialize( stream );
		}
	}
}