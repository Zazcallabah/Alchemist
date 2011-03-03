using System;
using System.Collections.Generic;
using System.Linq;
using Alchemist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// </summary>
	[TestClass]
	public class ChemistTests
	{
		[TestMethod]
		public void ChemistCanHandleAdditionOfNewElementLateInTheGame()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { "fire", ">water", ">air", "", "alcohol", "", "", "ocean", ">earth", "bunny", "+bunny", "bunny", "", "", "" } );

			c.Cook();

			Assert.AreEqual( 21, rs.Rules.Count() );
			Assert.AreEqual( rs.Rules.Distinct().Count(), rs.Rules.Count() );
		}

		[TestMethod]
		public void CanExitPrematurely()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { "fire", ">water", "!", "alcohol", "+alcohol", "ocean", "", "", "", "", "", "", "", "" } );

			c.Cook();

			Assert.AreEqual( 2, rs.FoundElements.Count() );
		}

		[TestMethod]
		public void CanPerformBasicSimulationRoundtrip()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { "fire", ">water", "", "alcohol", "+alcohol", "ocean", "", "", "", "", "", "", "", "" } );

			c.Cook();

			Assert.AreEqual( 4, rs.FoundElements.Count() );
			Assert.IsTrue( rs.Rules.Any( r => r.Result.Contains( new Element( "ocean" ) ) ) );
		}

		[TestMethod]
		public void UserCanExplicitlyAddRuleAndResult()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { "fire", ">water", "#water,water:alcohol", "!" } );

			c.Cook();

			Assert.AreEqual( 1, rs.Rules.Count( r => r.Equals( new Rule( new[] { "water", "water" } ) ) ) );
			Assert.AreEqual( "alcohol", rs.Rules.First( r => r.Equals( new Rule( new[] { "water", "water" } ) ) ).Result[0].Name );
		}

		static Chemist Setup( RuleSet ruleSet, IEnumerable<string> input )
		{
			var tc = new TestCommunicator();
			foreach( var s in input )
				tc.InputQueue.Enqueue( s );
			return new Chemist( new AlchemyController( ruleSet ), tc );
		}
	}

	public class TestCommunicator : ICommunicator
	{
		public event EventHandler<EventArgs> DisplayCalled;

		public void Display( string data )
		{
			if( null != DisplayCalled )
			{
				DisplayCalled( data, EventArgs.Empty );
			}
		}

		public Queue<string> InputQueue = new Queue<string>();

		public string GetInput()
		{
			return InputQueue.Count() == 0 ? string.Empty : InputQueue.Dequeue();
		}
	}
}
