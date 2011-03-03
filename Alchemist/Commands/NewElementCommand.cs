namespace Alchemist.Commands
{
	public class NewElementCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return input.TrimStart().StartsWith( ">" );
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			controller.RegisterNewElement( input.TrimStart( '>' ) );

			return Do.AnotherRule;
		}

		public override string ToString()
		{
			return "> - Adds a new basic element";
		}

		public int Priority
		{
			get { return 0; }
		}
	}
}
