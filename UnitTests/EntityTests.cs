using System.Linq;
using Alchemist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// Summary description for EntityTests
	/// </summary>
	[TestClass]
	public class EntityTests
	{
		[TestMethod]
		public void RulesCanReportThatItContainsElementInResult()
		{
			var t = new Rule( new[] { "u", "x" }, "a" );
			bool result = t.Contains( "a" );

			Assert.IsTrue( result );
		}
		[TestMethod]
		public void RulesCanReportThatItNotContainsElementNotInResult()
		{
			var t = new Rule( new[] { "u", "x" }, "a" );
			bool result = t.Contains( "33" );

			Assert.IsFalse( result );
		}
		[TestMethod]
		public void RulesCanReportThatItNotContainsElementNotInResultWithEmptyIngredients()
		{
			var t = new Rule( new string[0], "a" );
			bool result = t.Contains( "33" );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void RulesCanReportThatItContainsElementInResultWithEmptyIngredients()
		{
			var t = new Rule( new string[0], "a" );
			bool result = t.Contains( "a" );

			Assert.IsTrue( result );
		}


		[TestMethod]
		public void RulesCanReportThatItContainsElementInIngredients()
		{
			var t = new Rule( new[] { new Element( "a" ), new Element( "x" ) } );
			bool result = t.Contains( "a" );

			Assert.IsTrue( result );
		}
		[TestMethod]
		public void RulesCanReportThatItContainsElementInIngredients2()
		{
			var t = new Rule( new[] { new Element( "a" ), new Element( "x" ) } );
			bool result = t.Contains( "x" );

			Assert.IsTrue( result );
		}

		[TestMethod]
		public void RulesCanReportThatItNotContainsElementNotInIngredients()
		{
			var t = new Rule( new[] { new Element( "a" ), new Element( "x" ) } );
			bool result = t.Contains( "x3" );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void RulesCanReportThatItNotContainsElementEmptyIngredients()
		{
			var t = new Rule();
			bool result = t.Contains( "x3" );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void RuleAddsIngredientsInAlphabeticalOrder()
		{
			var t = new Rule( new[] { new Element( "a" ), new Element( "x" ) } );
			var t2 = new Rule( new[] { new Element( "x" ), new Element( "a" ) } );

			Assert.AreEqual( t2.Ingredients[0], t.Ingredients[0] );
		}



		[TestMethod]
		public void RuleCanParseInputLineWithMultipleResults()
		{
			const string inputline = "ao, ao1";
			var r = new Rule();
			r.SetResult( inputline );

			Assert.AreEqual( 2, r.Result.Count() );
			Assert.IsTrue( r.Result.Select( ra => ra.Name ).Any( re => re.Equals( "ao" ) ) );
			Assert.IsTrue( r.Result.Select( ra => ra.Name ).Any( re => re.Equals( "ao1" ) ) );
		}
		[TestMethod]
		public void RuleCanParseInputLine()
		{
			const string inputline = "ao ao";
			var r = new Rule();
			r.SetResult( inputline );

			Assert.AreEqual( inputline, r.Result.FirstOrDefault().Name );
		}

		[TestMethod]
		public void RulePrintsNiceMessage()
		{
			var r = new Rule( new[] { new Element( "a" ), new Element( "a" ) } );
			Assert.AreNotEqual( "Alchemist.Rule", r.ToString() );
		}

		[TestMethod]
		public void RuleWithoutResultIsConsideredEmpty()
		{
			var r = new Rule();
			var r1 = new Rule( new Element[0], new Element[0] );
			var r2 = new Rule( new Element[1], new Element[1] );
			var r3 = new Rule( new Element[] { null }, new Element[0] );

			Assert.IsTrue( r.IsEmpty );
			Assert.IsTrue( r1.IsEmpty );
			Assert.IsTrue( r2.IsEmpty );
			Assert.IsTrue( r3.IsEmpty );
		}

	}
}
