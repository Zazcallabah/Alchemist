using System;
using Alchemist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// Summary description for TimedRoundtripTests
	/// </summary>
	[TestClass]
	[Ignore]
	public class TimedRoundtripTests
	{
		[TestMethod]
		[DeploymentItem( "xmldata\\timingtest.xml" )]
		public void TimeMultiAddFunction()
		{
			var comm = new TestCommunicator();
			var input = new[] { ">anything", "*anything", "!" };
			foreach( var s in input )
				comm.InputQueue.Enqueue( s );
			var rs = GetFromXml( "timingtest.xml" );
			var controller = new AlchemyController( rs );
			var chemist = new Chemist( controller, comm );
			long mark = 0;
			comm.DisplayCalled += ( s, e ) =>
				{
					if( s.ToString().Contains( "..." ) )
						mark = DateTime.Now.Ticks;
				};
			chemist.Cook();
			var time = new TimeSpan( DateTime.Now.Ticks - mark );
			Assert.Fail( "Took: " + time );
		}

		[TestMethod]
		[DeploymentItem( "xmldata\\timingtest.xml" )]
		public void TimeSecondStart()
		{
			var comm = new TestCommunicator();
			var input = new[] { ">anything", "+anything", "!" };
			foreach( var s in input )
				comm.InputQueue.Enqueue( s );
			var rs = GetFromXml( "timingtest.xml" );
			var controller = new AlchemyController( rs );
			var chemist = new Chemist( controller, comm );
			long mark = 0;
			comm.DisplayCalled += ( s, e ) =>
				{
					if( s.ToString().Contains( "..." ) )
						mark = DateTime.Now.Ticks;
				};
			chemist.Cook();
			var time = new TimeSpan( DateTime.Now.Ticks - mark );
			Assert.Fail( "Took: " + time );
		}

		[TestMethod]
		[DeploymentItem( "xmldata\\timingtest.xml" )]
		public void TimeLotsOfNullReturns()
		{
			var comm = new TestCommunicator();
			var input = new[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "!" };
			foreach( var s in input )
				comm.InputQueue.Enqueue( s );
			var rs = GetFromXml( "timingtest.xml" );
			Attach( rs );
			var controller = new AlchemyController( rs );
			var chemist = new Chemist( controller, comm );
			long mark = 0;
			comm.DisplayCalled += ( s, e ) =>
				{
					if( s.ToString().Contains( "algae + fire elemental" ) )
						mark = DateTime.Now.Ticks;
				};
			chemist.Cook();
			var time = new TimeSpan( DateTime.Now.Ticks - mark );
			Assert.Fail( "Took: " + time );
		}

		static XmlPersister Attach( RuleSet rs )
		{
			var tempfilename = new Random().Next( 100000 ) + "-data.xml";
			var persister = new XmlPersister( new RuleSetXmlSerializer(), new StreamFactory( tempfilename ) );
			persister.RegisterRuleSet( rs );
			return persister;
		}

		[TestMethod]
		[DeploymentItem( "xmldata\\timingtest.xml" )]
		public void TimeInitialStart()
		{
			var comm = new TestCommunicator();
			comm.InputQueue.Enqueue( "!" );
			var rs = GetFromXml( "timingtest.xml" );
			var controller = new AlchemyController( rs );
			var chemist = new Chemist( controller, comm );

			PerformTimedTask( chemist.Cook );
		}

		[TestMethod]
		[DeploymentItem( "xmldata\\timingtest.xml" )]
		public void TimeEntireDeserialization()
		{
			PerformTimedTask( () => GetFromXml( "timingtest.xml" ) );
		}

		static void PerformTimedTask( Action task )
		{
			long mark = DateTime.Now.Ticks;
			task();
			var time = new TimeSpan( DateTime.Now.Ticks - mark );
			Assert.Fail( "Took: " + time );
		}

		static RuleSet GetFromXml( string filename )
		{
			var serializer = new RuleSetXmlSerializer();
			var factory = new StreamFactory( filename );
			var persister = new XmlPersister( serializer, factory );
			return persister.RecreateRuleSet();
		}
	}
}
