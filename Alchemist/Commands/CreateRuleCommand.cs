namespace Alchemist.Commands
{
	public class CreateRuleCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return input != null;
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			var rule = controller.RecommendNewRule();
			rule.SetResult( input );
			controller.ReportChangedRule( rule );

			return Do.AnotherRule;
		}

		public override string ToString()
		{
			return "";
		}

		public int Priority
		{
			get { return -1; }
		}
	}
}
