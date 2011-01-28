using Alchemist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// Summary description for EqualityTests
	/// </summary>
	[TestClass]
	public class EqualityTests
	{
		[TestMethod]
		public void RulesAreNotEqualIfTheyHaveDifferentIngredientsAndNoResult()
		{
			var r1 = new Rule( new[] { new Element( "1" ), new Element( "1" ) } );
			var r2 = new Rule( new[] { new Element( "1" ), new Element( "a" ) } );

			Assert.AreNotEqual( r1, r2 );
		}

		[TestMethod]
		public void TwoEqualElementsAreEqual()
		{
			var e1 = new Element( "fire" );
			var e2 = new Element( "fire" );

			Assert.AreEqual( e1, e2 );
		}

		[TestMethod]
		public void RulesAreNotEqualIfTheyHaveEntirelyDifferentIngredientsAndNoResult()
		{
			var r1 = new Rule( new[] { new Element( "1" ), new Element( "2" ) } );
			var r2 = new Rule( new[] { new Element( "a" ), new Element( "k" ) } );

			Assert.AreNotEqual( r1, r2 );
		}

		[TestMethod]
		public void RulesAreEqualIfTheyHaveSameIngredients()
		{
			var r1 = new Rule( new[] { new Element( "1" ), new Element( "2" ) } );
			var r2 = new Rule( new[] { new Element( "1" ), new Element( "2" ) } );

			Assert.AreEqual( r1, r2 );
		}

		[TestMethod]
		public void RulesAreNotEqualIfTheyHaveDifferentIngredients()
		{
			var r1 = new Rule( new[] { "a" }, "aoeu" );
			var r2 = new Rule( new[] { "1" }, "aoeuoo" );

			Assert.AreNotEqual( r1, r2 );
		}

		[TestMethod]
		public void RuleEqualityOnlyCaresAboutIngredients()
		{
			var r1 = new Rule( new[] { "a" }, "aoeu" );
			var r2 = new Rule( new[] { "a" }, "aoeuoo" );

			Assert.AreEqual( r1, r2 );
		}

		[TestMethod]
		public void RuleEqualityDontCareAboutIngredientOrder()
		{
			var r1 = new Rule( new[] { "a", "aoeu" }, new string[0] );
			var r2 = new Rule( new[] { "aoeu", "a" }, new string[0] );

			Assert.AreEqual( r1, r2 );
		}

		[TestMethod]
		public void TwoRuleSetsWithDifferentRulesAreNotEqual()
		{
			var rs1 = new RuleSet { Rules = new[] { new Rule( new[] { "b" } ) }, FoundElements = new[] { new Element( "a" ) } };
			var rs2 = new RuleSet { Rules = new[] { new Rule( new[] { "a" } ) }, FoundElements = new[] { new Element( "a" ) } };

			Assert.AreNotEqual( rs1, rs2 );
		}

		[TestMethod]
		public void TwoUnequalRuleSetsAreNotEqual()
		{
			var rs1 = new RuleSet { FoundElements = new[] { new Element( "a" ) } };
			var rs2 = new RuleSet { FoundElements = new[] { new Element( "b" ) } };

			Assert.AreNotEqual( rs1, rs2 );
		}
		[TestMethod]
		public void TwoEqualRuleSetsAreEqual()
		{

			var rs1 = new RuleSet { FoundElements = new[] { new Element( "a" ) } };
			var rs2 = new RuleSet { FoundElements = new[] { new Element( "a" ) } };

			Assert.AreEqual( rs1, rs2 );
			Assert.AreNotSame( rs1, rs2 );
		}
	}
}
