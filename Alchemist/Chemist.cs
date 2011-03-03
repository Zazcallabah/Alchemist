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
				new CreateRuleCommand(),
				new AddSpecificRuleCommand()
			}.OrderByDescending( c => c.Priority );
		}

		public void Cook()
		{
			PrintMenu();
			if( _controller.State == AlchemyState.NotStarted )
				_communicator.Display( "No elements recorded, please add basic elements to combine." );

			while( true )
			{
				foreach( var c in _preCommands )
				{
					var result = c.Run( _controller, _communicator );
					if( result == Do.Exit )
						return;
				}

				var reportback = _communicator.GetInput();

				// Special case for first entered element
				if( _controller.State == AlchemyState.NotStarted )
				{
					var c = new NewElementCommand();
					if( !string.IsNullOrEmpty( reportback ) )
						c.Run( reportback, _controller, _communicator );
					continue;
				}

				foreach( var c in _commands.Where( c => c.AppliesTo( reportback ) ) )
				{
					var result = c.Run( reportback, _controller, _communicator );
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
		}
	}
}
