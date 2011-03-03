namespace Alchemist.Commands
{
	public class FinalizeElementCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return input.TrimStart().StartsWith( "+" );
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			controller.FinalizeElement( input.TrimStart( '+' ) );

			return Do.AnotherRule;
		}

		public override string ToString()
		{
			return "+ - Sets an existing element to 'finalized'";
		}

		public int Priority
		{
			get { return 0; }
		}
	}
}
