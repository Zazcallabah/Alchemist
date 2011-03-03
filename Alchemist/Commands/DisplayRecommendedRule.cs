namespace Alchemist.Commands
{
	public class DisplayRecommendedRule : IPreCommand
	{
		public Do Run( AlchemyController controller, ICommunicator communicator )
		{
			var rule = controller.RecommendNewRule();
			if( rule != null && rule != Rule.EmptyRule )
				communicator.Display( rule.ToString() );
			return rule == Rule.EmptyRule ? Do.Exit : Do.KeepProcessing;
		}

		public int Priority
		{
			get { return 99; }
		}
	}
}