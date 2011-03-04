using Alchemist;
using Alchemist.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// Summary description for CommandTest
	/// </summary>
	[TestClass]
	public class CommandTest
	{
		[TestMethod]
		public void GraphCommandExists()
		{
			ICommand command = new GraphCommand();

			Assert.IsTrue( command.AppliesTo( "@fire" ) );
		}

		[TestMethod]
		public void PrintCommandHasPrint()
		{
			ICommand command = new PrintCommand();

			Assert.IsTrue( command.AppliesTo( "?fire" ) );
		}

		[TestMethod]
		public void PrintCommandCanPrint()
		{
			ICommand command = new PrintCommand();
			var comm = new TestCommunicator();
			int count = 0;
			comm.DisplayCalled += ( a, e ) =>
			{
				count++;
			};
			var rs = new RuleSet()
				{
					Rules = new[] { new Rule( new[] { "fire", "water" }, "water" ) }
				};
			var controller = new AlchemyController( rs );
			command.Run( "?fire", controller, comm );

			Assert.AreEqual( 1, count );
		}

		[TestMethod]
		public void PrintCommandWontPrintEmpty()
		{
			ICommand command = new PrintCommand();
			var comm = new TestCommunicator();
			int count = 0;
			comm.DisplayCalled += ( a, e ) =>
			{
				count++;
			};
			var rs = new RuleSet()
				{
					Rules = new[] { new Rule( new[] { "fire", "water" } ) }
				};
			var controller = new AlchemyController( rs );
			command.Run( "?fire", controller, comm );

			Assert.AreEqual( 0, count );

		}

		[TestMethod]
		public void PrintCommandWontPrintIrrelevant()
		{
			ICommand command = new PrintCommand();
			var comm = new TestCommunicator();
			int count = 0;
			comm.DisplayCalled += ( a, e ) =>
			{
				count++;
			};
			var rs = new RuleSet()
				{
					Rules = new[] { new Rule( new[] { "fire", "water" }, "water" ) }
				};
			var controller = new AlchemyController( rs );
			command.Run( "?fire3", controller, comm );

			Assert.AreEqual( 0, count );

		}

		[TestMethod]
		public void CommandCanAddRulesForEveryPossibleCombinationForAnElement()
		{
			ICommand command = new AddMultiComboCommand();

			Assert.IsTrue( command.AppliesTo( "*fire" ) );
		}

		[TestMethod]
		public void MultiCommandAddsRules()
		{
			ICommand command = new AddMultiComboCommand();
			var com = new TestCommunicator();
			var rs = new RuleSet()
			{
				FoundElements = new[] { new Element( "fire" ), new Element( "water" ) }
			};

			var c = new AlchemyController( rs );
			command.Run( "*fire", c, com );

			Assert.AreEqual( 2, rs.Rules.Length );
		}

		[TestMethod]
		public void CommandHasIsCommandMethod()
		{
			ICommand command = new TestCommand();

			Assert.IsTrue( command.AppliesTo( "any string" ) );
		}
		[TestMethod]
		public void CommandHasPriority()
		{
			ICommand command = new TestCommand();
			Assert.AreEqual( 0, command.Priority );
		}

		[TestMethod]
		public void CanRunCommand()
		{
			ICommand command = new TestCommand();
			ICommunicator com = new TestCommunicator();
			AlchemyController con = new AlchemyController( new RuleSet() );
			command.Run( "input", con, com );
		}

		[TestMethod]
		public void CanGetInstructions()
		{
			ICommand com = new TestCommand();
			Assert.AreNotEqual( "UnitTests.TestCommand", com.ToString() );
		}

		[TestMethod]
		public void ExitCommandExists()
		{
			ICommand com = new ExitCommand();
			Assert.IsTrue( com.AppliesTo( "! " ) );
		}

		[TestMethod]
		public void ExitCommandReturnsExitDirectiveWhenRun()
		{
			var c = new ExitCommand();
			var result = c.Run( "", new AlchemyController( new RuleSet() ), new TestCommunicator() );
			Assert.AreEqual( Do.Exit, result );
		}

		[TestMethod]
		public void CommandExistsForCheckingIfControllerIsDone()
		{
			var c = new IsFinishedCommand();
		}

		[TestMethod]
		public void IsFinishedCommandSaysExitWhenControllerIsDone()
		{
			var c = new IsFinishedCommand();
			var rs = new RuleSet() { FoundElements = TestTools.GenerateElements( 2 ), Rules = TestTools.GenerateRules( 3 ) };
			var a = new AlchemyController( rs );
			Assert.AreEqual( Do.Exit, c.Run( a, new TestCommunicator() ) );
		}

		[TestMethod]
		public void IsFinishedCommandSaysContinueIfNotDone()
		{
			var c = new IsFinishedCommand();
			var rs = new RuleSet() { FoundElements = TestTools.GenerateElements( 2 ), Rules = TestTools.GenerateRules( 2 ) };
			var a = new AlchemyController( rs );
			Assert.AreEqual( Do.KeepProcessing, c.Run( a, new TestCommunicator() ) );
		}

		[TestMethod]
		public void IsFinishedCommandSaysContinueIfEmptyElementList()
		{
			var c = new IsFinishedCommand();
			var rs = new RuleSet();
			var a = new AlchemyController( rs );
			Assert.AreEqual( Do.KeepProcessing, c.Run( a, new TestCommunicator() ) );
		}

		[TestMethod]
		public void DisplayRuleCommandExists()
		{
			IPreCommand c = new DisplayRecommendedRule();
		}

		[TestMethod]
		public void DisplayRuleDisplaysNothingIfApplicable()
		{
			IPreCommand c = new DisplayRecommendedRule();
			var rs = new RuleSet();
			var a = new AlchemyController( rs );
			var t = new TestCommunicator();
			t.DisplayCalled += ( o, e ) => Assert.IsTrue( string.IsNullOrEmpty( (string) o ) );
			var result = c.Run( a, t );
			Assert.AreEqual( Do.KeepProcessing, result );
		}

		[TestMethod]
		public void DisplayRuleDisplaysRuleIfApplicable()
		{
			IPreCommand c = new DisplayRecommendedRule();
			var rs = new RuleSet() { FoundElements = TestTools.GenerateElements( 1 ) };
			var a = new AlchemyController( rs );
			var t = new TestCommunicator();
			int count = 0;
			t.DisplayCalled += ( o, e ) =>
			{
				count++;
				Assert.IsTrue( ( (string) o ).Contains( "+" ) );
			};
			var result = c.Run( a, t );
			Assert.AreEqual( Do.KeepProcessing, result );
			Assert.AreEqual( 1, count );
		}

		[TestMethod]
		public void ExistsValidateInputCommand()
		{
			ICommand c = new ValidateInput();
			Assert.AreEqual( Do.KeepProcessing, c.Run( "", null, null ) );
			Assert.AreEqual( Do.KeepProcessing, c.Run( "aoeu", null, null ) );
			Assert.AreEqual( Do.Exit, c.Run( null, null, null ) );
		}
	}


	public class TestCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return true;
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			return Do.AnotherRule;
		}
		public int Priority { get { return 0; } }
		public override string ToString()
		{
			return "any thing";
		}
	}
}
