using System;

namespace Alchemist.Commands
{
	public class PrintCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return input.TrimStart().StartsWith( "?" );
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			string elementname = input.TrimStart( '?' );

			controller.ForEachRuleContaining( elementname, ( r ) =>
			{
				if( r.Result.Length > 0 )
					communicator.Display( r.ToString() );
			} );

			return Do.AnotherRule;
		}

		public override string ToString()
		{
			return "? - Display info on element";
		}

		public int Priority
		{
			get { return 0; }
		}
	}
}
