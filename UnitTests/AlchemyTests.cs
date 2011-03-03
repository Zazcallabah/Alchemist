using System.Collections.Generic;
using System.Linq;
using Alchemist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// Summary description for AlchemyTests
	/// </summary>
	[TestClass]
	public class AlchemyTests
	{
		[TestMethod]
		public void ExtensiveControllerRoundtripMonkeyTest()
		{
			var rs = new RuleSet();
			var c = new AlchemyController( rs );
			c.RegisterNewElement( "fire" );
			c.RegisterNewElement( "water" );
			c.RegisterNewElement( "burn" );
		}

		//[TestMethod]
		//public void ControllerAccountsForFinalizedElementsWhenCalculatingStateFinished()
		//{
		//    var rs = new RuleSet()
		//    {
		//        FoundElements = new[] { new Element( "fire" ), new Element( "water" ) { TerminalValue = true } },
		//        Rules = new[] { new Rule
		//        {
		//            Ingredients = new [] { new Element( "fire" ), new Element("fire") }
		//        }}

		//    };
		//    var c = new AlchemyController( rs );

		//    Assert.AreEqual( AlchemyState.Finished, c.State );
		//}

		[TestMethod]
		public void ControllerCanSayIfHasntStartedYet()
		{
			var rs = new RuleSet();
			var c = new AlchemyController( rs );

			Assert.AreEqual( AlchemyState.NotStarted, c.State );
		}

		[TestMethod]
		public void ControllerCanSayThatItHasStarted()
		{
			var rs = new RuleSet() { FoundElements = new[] { new Element( "fire" ) } };
			var c = new AlchemyController( rs );

			Assert.AreEqual( AlchemyState.Started, c.State );
		}

		[TestMethod]
		public void ControllerCanSayThatItHasFinished()
		{
			var rs = new RuleSet()
			{
				FoundElements = new[] { new Element( "fire" ) },
				Rules = new[] { new Rule
				{
					Ingredients = new [] { new Element( "fire" ), new Element("fire") }
				}}

			};
			var c = new AlchemyController( rs );

			Assert.AreEqual( AlchemyState.Finished, c.State );
		}

		[TestMethod]
		public void ControllerCanSayThatItHasFinishedWithComplexRuleset()
		{
			TestControllerForRules( 1, 1 );
			TestControllerForRules( 1, 2 );
			TestControllerForRules( 2, 3 );
			TestControllerForRules( 2, 7 );
			TestControllerForRules( 3, 6 );
			TestControllerForRules( 4, 10 );
			TestControllerForRules( 100, 5050 );
		}

		void TestControllerForRules( int elements, int rules )
		{
			var rs = new RuleSet()
			{
				FoundElements = TestTools.GenerateElements( elements ),
				Rules = TestTools.GenerateRules( rules )
			};

			var c = new AlchemyController( rs );

			Assert.AreEqual( AlchemyState.Finished, c.State );
		}



		[TestMethod]
		public void CanRecommendRuleForControllerToPrioritizeInRecomendations()
		{
			var rs = new RuleSet();
			var c = new AlchemyController( rs );
			c.RegisterNewElement( "fire" );
			c.RegisterNewElement( "water" );
			c.RegisterNewElement( "burn" );

			var r = c.RecommendNewRule( "burn", "water" );

			Assert.AreNotEqual( new Rule( new[] { "fire", "fire" } ), r );
			Assert.AreEqual( new Rule( new[] { "burn", "water" } ), r );
		}

		[TestMethod]
		public void IgnoresInputWhenRecommendingNonexistentElements()
		{
			var rs = new RuleSet();
			var c = new AlchemyController( rs );
			c.RegisterNewElement( "fire" );
			c.RegisterNewElement( "water" );
			c.RegisterNewElement( "burn" );

			var r = c.RecommendNewRule( "burn", "aoeu" );

			Assert.AreEqual( new Rule( new[] { "fire", "fire" } ), r );
		}

		[TestMethod]
		public void OrderDoesntmatterForPrefferedRule()
		{
			var rs = new RuleSet();
			var c = new AlchemyController( rs );
			c.RegisterNewElement( "fire" );
			c.RegisterNewElement( "water" );
			c.RegisterNewElement( "burn" );

			var r = c.RecommendNewRule( "burn", "water" );
			var r2 = c.RecommendNewRule( "water", "burn" );

			Assert.AreEqual( r2, r );
		}

		[TestMethod]
		public void ControllerRecommendsCombinationsButNotDuplicates()
		{
			var rs = new RuleSet();
			var c = new AlchemyController( rs );
			c.RegisterNewElement( "a" );
			c.RegisterNewElement( "mb" );
			c.RegisterNewElement( "aoeu" );
			c.ReportChangedRule( c.RecommendNewRule() );

			var rules = new List<Rule>();
			for( int i = 0; i < 100; i++ )
			{
				var r1 = c.RecommendNewRule();
				if( r1 == Rule.EmptyRule )
					break;
				rules.Add( r1 );
				c.ReportChangedRule( r1 );
			}

			Assert.AreEqual( 5, rules.Count );
		}

		[TestMethod]
		public void ControllerRecommendsAllPossibleCombinationsOfElementsLarge()
		{
			var rs = new RuleSet();
			var c = new AlchemyController( rs );
			c.RegisterNewElement( "a" );
			c.RegisterNewElement( "mb" );
			c.RegisterNewElement( "aoeu" );
			var rules = new List<Rule>();
			for( int i = 0; i < 100; i++ )
			{
				var r1 = c.RecommendNewRule();
				if( r1 == Rule.EmptyRule )
					break;
				rules.Add( r1 );
				c.ReportChangedRule( r1 );
			}

			Assert.AreEqual( 6, rules.Count );
		}

		[TestMethod]
		public void ControllerRecommendsAllPossibleCombinationsOfElements()
		{
			const string e1 = "1";
			const string e2 = "2";
			var rs = new RuleSet();
			var c = new AlchemyController( rs );
			c.RegisterNewElement( e1 );
			c.RegisterNewElement( e2 );
			var rules = new List<Rule>();
			for( int i = 0; i < 100; i++ )
			{
				var r1 = c.RecommendNewRule();
				if( r1 == Rule.EmptyRule )
					break;
				rules.Add( r1 );
				c.ReportChangedRule( r1 );
			}

			Assert.AreEqual( 3, rules.Count );
			Assert.IsTrue( AssertForCombo( e1, e1, rules ) );
			Assert.IsTrue( AssertForCombo( e1, e2, rules ) );
			Assert.IsTrue( AssertForCombo( e2, e2, rules ) );

		}

		static bool AssertForCombo( string e1, string e2, IEnumerable<Rule> list )
		{
			var t = new Rule( new[] { new Element( e1 ), new Element( e2 ) } );
			return list.Any( r => r.Equals( t ) );
		}

		[TestMethod]
		public void AlchemyControllerWontReccomendRuleWithTerminalElements()
		{
			var rs = new RuleSet();
			var a = new AlchemyController( rs );

			a.RegisterNewElement( "a" );
			a.RegisterNewElement( "b" );
			a.RegisterNewElement( "c" );
			a.FinalizeElement( "a" );


			var rules = new List<Rule>();
			for( int i = 0; i < 100; i++ )
			{
				var r1 = a.RecommendNewRule();
				if( r1 == Rule.EmptyRule )
					break;
				rules.Add( r1 );
				a.ReportChangedRule( r1 );
			}

			Assert.AreEqual( 3, rules.Count );
			Assert.IsFalse( rules.Any( r => r.Ingredients.Any( i => i.Name.Equals( a ) ) ) );
		}

		[TestMethod]
		public void AlchemyControllerCanMarkElementAsTerminal()
		{
			var rs = new RuleSet();
			var a = new AlchemyController( rs );

			a.RegisterNewElement( "a" );
			a.RegisterNewElement( "b" );
			a.RegisterNewElement( "c" );
			a.RegisterNewElement( "d" );
			a.RegisterNewElement( "e" );

			a.FinalizeElement( "c" );

			var em = rs.FoundElements.FirstOrDefault( e => e.Name.Equals( "c" ) );

			Assert.IsTrue( em.TerminalSpecified );
			Assert.IsTrue( em.TerminalValue.HasValue );
			if( em.TerminalValue != null )
			{
				Assert.IsTrue( em.TerminalValue.Value );
			}
			Assert.AreEqual( 1, rs.FoundElements.Count( e => e.TerminalValue.HasValue && e.Terminal ) );
			Assert.AreEqual( 4, rs.FoundElements.Count( e => !e.TerminalValue.HasValue ) );

		}

		[TestMethod]
		public void AlchemyControllerCanAddNewElement()
		{
			var rs = new RuleSet();
			var controller = new AlchemyController( rs );
			const string element = "aoeu";
			controller.RegisterNewElement( element );

			Assert.AreEqual( 1, rs.FoundElements.Count() );
			Assert.AreEqual( element, rs.FoundElements.First().Name );
		}

		[TestMethod]
		public void AlchemyControllerRecommendsEmptyRuleIfDone()
		{
			var rs = new RuleSet();
			var controller = new AlchemyController( rs );
			controller.RegisterNewElement( "fire" );
			var rule = controller.RecommendNewRule();
			rs.Rules = new[] { rule };

			var finalRule = controller.RecommendNewRule();

			Assert.AreEqual( Rule.EmptyRule, finalRule );
		}

		[TestMethod]
		public void CanReportTestedRuleBackToController()
		{
			var rs = new RuleSet();
			var controller = new AlchemyController( rs );
			controller.RegisterNewElement( "fire" );
			var rule = controller.RecommendNewRule();
			rule.Result = new[] { new Element( "water" ) };

			controller.ReportChangedRule( rule );

			Assert.AreEqual( 1, rs.Rules.Count() );
			Assert.AreEqual( 2, rs.FoundElements.Count() );

		}

		[TestMethod]
		public void AlchemyControllerRecommendsRule()
		{
			var rs = new RuleSet
			{
				FoundElements = new[] { new Element( "fire" ) }
			};
			var controller = new AlchemyController( rs );

			var rule = controller.RecommendNewRule();

			Assert.IsNotNull( rule );
			Assert.AreEqual( new Rule( new[] { new Element( "fire" ), new Element( "fire" ) } ), rule );
		}

		[TestMethod]
		public void AlchemyControllerRecommendsNullIfNoElements()
		{
			var rs = new RuleSet();
			var controller = new AlchemyController( rs );

			var rule = controller.RecommendNewRule();

			Assert.IsNull( rule );
		}
	}
}
