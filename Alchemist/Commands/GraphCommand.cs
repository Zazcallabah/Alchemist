using System;

namespace Alchemist.Commands
{
	public class GraphCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return input.TrimStart().StartsWith( "@" );
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			//
			return Do.AnotherRule;
		}

		public override string ToString()
		{
			return "@ - Displays combination tree for element";
		}

		public int Priority
		{
			get { return 0; }
		}
	}
}
