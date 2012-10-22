using System;
using System.Collections.Generic;
using System.IO;
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
		[DeploymentItem( "xmldata\\testdataset2.xml" )]
		public void AddingSelfReferentialRuleWillNotCrash()
		{
			var rs = TestTools.GetFromXml( "testdataset2.xml" );
			var c = Setup( rs, new[] { "#man,vampire:vampire,vampire", "!" } );
			c.Cook();
		}


		[TestMethod]
		public void CanUsePrintCommand()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", "energy", "?fire" } );
			c.Cook();

			Assert.AreEqual( 3, rs.FoundElements.Length );
		}

		[TestMethod]
		public void AddExplicitRuleWithNewElementsResetsRecommendedElementProgress()
		{
			var rs = new RuleSet();
			var controller = new AlchemyController( rs );
			var c = Setup( controller, new[] { ">fire", ">water", "", "", "#fire,fire:energy", "!" } );
			c.Cook();

			var rule = controller.RecommendNewRule();

			Assert.AreEqual( new Rule( new[] { "fire", "energy" } ), rule );
		}

		[TestMethod]
		public void MultiAddDoesntWorkWithUnknownElement()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", ">air", ">earth", "*trolololo", "!" } );

			c.Cook();

			Assert.AreEqual( 0, rs.Rules.Count() );
			Assert.AreEqual( 4, rs.FoundElements.Count() );
		}

		[TestMethod]
		public void MultiAddDoesntOverrideExistingRule()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", ">air", ">earth", "#fire,water:water", "*water", "!" } );

			c.Cook();

			Assert.AreEqual( 4, rs.Rules.Count() );
			Assert.AreEqual( 1, rs.Rules.First( r => r.Equals( new Rule( new[] { "water", "fire" } ) ) ).Result.Count() );
		}

		[TestMethod]
		public void BadDataAsIngredientsDoesntCompletelyBorktheruleSet()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", "#fire,f3ire:water", "!" } );

			c.Cook();

			Assert.AreEqual( 0, rs.Rules.Count() );
			Assert.AreEqual( 1, rs.FoundElements.Count() );
		}

		[TestMethod]
		public void NewElementsCanBeAddedUsingSpecificRuleAddCommand()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", "#fire,fire:water", "!" } );

			c.Cook();

			Assert.AreEqual( 1, rs.Rules.Count() );
			Assert.AreEqual( 2, rs.FoundElements.Count() );
			Assert.AreEqual( 1, rs.FoundElements.Count( e => e.Equals( new Element( "water" ) ) ) );
		}

		[TestMethod]
		public void UserCanSetSpecificRule()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", ">air", "#fire,air:water", "!" } );

			c.Cook();

			Assert.AreEqual( 1, rs.Rules.Count() );
			Assert.AreEqual( 1, rs.Rules.Count( r => new Rule( new[] { "fire", "air" } ).Equals( r ) && r.Result.Count() == 1 && r.Result[0].Name.Equals( "water" ) ) );
		}

		[TestMethod]
		public void SpecificRuleOverridesExistingRule()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", ">air", "#fire,air:water", "#fire,air:energy", "!" } );

			c.Cook();

			Assert.AreEqual( 1, rs.Rules.Count() );
			Assert.AreEqual( 1, rs.Rules.Count( r => new Rule( new[] { "fire", "air" } ).Equals( r ) && r.Result.Count() == 1 && r.Result[0].Name.Equals( "energy" ) ) );
		}

		[TestMethod]
		public void ChemistCanHandleMultiAddCommand()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", ">air", "*fire", "!" } );

			c.Cook();

			Assert.AreEqual( 3, rs.Rules.Count() );
			Assert.AreEqual( 1, rs.Rules.Count( r => new Rule( new[] { "fire", "fire" } ).Equals( r ) && r.Result.Count() == 0 ) );
			Assert.AreEqual( 1, rs.Rules.Count( r => new Rule( new[] { "fire", "water" } ).Equals( r ) && r.Result.Count() == 0 ) );
			Assert.AreEqual( 1, rs.Rules.Count( r => new Rule( new[] { "fire", "air" } ).Equals( r ) && r.Result.Count() == 0 ) );
		}

		[TestMethod]
		public void ChemistCanHandleAdditionOfNewElementLateInTheGame()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", ">air", "", "alcohol", "", "", "ocean", ">earth", "bunny", "+bunny", "bunny", "", "", "" } );

			c.Cook();

			Assert.AreEqual( 21, rs.Rules.Count() );
			Assert.AreEqual( rs.Rules.Distinct().Count(), rs.Rules.Count() );
		}

		[TestMethod]
		public void CanExitPrematurely()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", "!", "alcohol", "+alcohol", "ocean", "", "", "", "", "", "", "", "" } );

			c.Cook();

			Assert.AreEqual( 2, rs.FoundElements.Count() );
		}

		[TestMethod]
		public void CanPerformBasicSimulationRoundtrip()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", "", "alcohol", "+alcohol", "ocean", "", "", "", "", "", "", "", "" } );

			c.Cook();

			Assert.AreEqual( 4, rs.FoundElements.Count() );
			Assert.IsTrue( rs.Rules.Any( r => r.Result.Contains( new Element( "ocean" ) ) ) );
		}

		[TestMethod]
		public void UserCanExplicitlyAddRuleAndResult()
		{
			var rs = new RuleSet();
			var c = Setup( rs, new[] { ">fire", ">water", "#water,water:alcohol", "!" } );

			c.Cook();

			Assert.AreEqual( 1, rs.Rules.Count( r => r.Equals( new Rule( new[] { "water", "water" } ) ) ) );
			Assert.AreEqual( "alcohol", rs.Rules.First( r => r.Equals( new Rule( new[] { "water", "water" } ) ) ).Result[0].Name );
		}

		static Chemist Setup( RuleSet ruleSet, IEnumerable<string> input )
		{
			return Setup( new AlchemyController( ruleSet ), input );
		}

		static Chemist Setup( AlchemyController controller, IEnumerable<string> input )
		{
			var tc = new TestCommunicator();
			foreach( var s in input )
				tc.InputQueue.Enqueue( s );
			return new Chemist( controller, tc );
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
