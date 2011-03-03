using System;

namespace Alchemist.Commands
{
	public class ExitCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return input.TrimStart().StartsWith( "!" );
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			communicator.Display( "Exit." );
			return Do.Exit;
		}

		public override string ToString()
		{
			return "! - Exits application";
		}

		public int Priority
		{
			get { return 0; }
		}
	}
}
