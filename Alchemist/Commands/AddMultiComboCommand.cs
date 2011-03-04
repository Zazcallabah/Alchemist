using System;

namespace Alchemist.Commands
{
	public class AddMultiComboCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return input.TrimStart().StartsWith( "*" );
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			string elementname = input.TrimStart( '*' );
			if( controller.ElementExists( elementname ) )
				controller.ForeachNonterminalElement( ( e ) => controller.ReportChangedRule( new Rule( new[] { elementname, e.Name } ) ) );
			return Do.AnotherRule;
		}

		public int Priority
		{
			get { return 0; }
		}

		public override string ToString()
		{
			return "* - add empty rule for every currently existing combo";
		}
	}
}