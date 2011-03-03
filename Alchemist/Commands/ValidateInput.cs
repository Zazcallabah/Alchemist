namespace Alchemist.Commands
{
	public class ValidateInput : ICommand
	{
		public bool AppliesTo( string input )
		{
			return true;
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			return input == null ? Do.Exit : Do.KeepProcessing;
		}

		public int Priority
		{
			get { return 50; }
		}

		public override string ToString()
		{
			return "";
		}
	}
}