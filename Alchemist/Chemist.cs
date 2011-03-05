using System.Collections.Generic;
using System.Linq;
using Alchemist.Commands;

namespace Alchemist
{
	public class Chemist
	{
		readonly AlchemyController _controller;
		readonly ICommunicator _communicator;
		readonly IOrderedEnumerable<ICommand> _commands;
		readonly IOrderedEnumerable<IPreCommand> _preCommands;

		public Chemist( AlchemyController controller, ICommunicator communicator )
		{
			_controller = controller;
			_communicator = communicator;

			_preCommands = new List<IPreCommand>
			{
				new IsFinishedCommand(),
				new DisplayRecommendedRule(),
			}.OrderByDescending( c => c.Priority );

			_commands = new List<ICommand>
			{
				new ValidateInput(),
				new ExitCommand(),
				new FinalizeElementCommand(),
				new NewElementCommand(),
				new AddMultiComboCommand(),
				new PrintCommand(),
				new CreateRuleCommand(),
				new AddSpecificRuleCommand()
			}.OrderByDescending( c => c.Priority );
		}

		public void Cook()
		{
			PrintMenu();

			while( true )
			{
				if( _preCommands.Select( c => c.Run( _controller, _communicator ) ).Any( result => result == Do.Exit ) )
					return;

				var reportback = _communicator.GetInput();

				foreach( var result in _commands
					.Where( c => c.AppliesTo( reportback ) )
					.Select( c => c.Run( reportback, _controller, _communicator ) ) )
				{
					if( result == Do.Exit )
						return;
					if( result == Do.AnotherRule )
						break;
				}

				_communicator.Display( "..." );
			}
		}

		void PrintMenu()
		{
			foreach( var c in _commands )
				_communicator.Display( c.ToString() );

			if( _controller.State == AlchemyState.NotStarted )
				_communicator.Display( "No elements recorded, please add basic elements to combine." );
		}
	}
}
