
namespace Alchemist.Commands
{
	public class AddSpecificRuleCommand : ICommand
	{
		public bool AppliesTo( string input )
		{
			return input.TrimStart().StartsWith( "#" );
		}

		public Do Run( string input, AlchemyController controller, ICommunicator communicator )
		{
			var splitinput = input.TrimStart( ' ', '#' ).Split( ':' );
			if( splitinput.Length < 2 )
				return Do.AnotherRule;

			var ingredients = splitinput[0].Split( ',' );

			var rule = new Rule( ingredients );
			rule.SetResult( splitinput[1] );
			controller.ReportChangedRule( rule );

			return Do.AnotherRule;
		}

		public override string ToString()
		{
			return "# - Register rule using pattern \"#ingredient,ingredient:[result[,result]]\"";
		}

		public int Priority
		{
			get { return 0; }
		}
	}
}
