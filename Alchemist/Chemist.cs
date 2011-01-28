namespace Alchemist
{
	public class Chemist
	{
		readonly AlchemyController _controller;
		readonly ICommunicator _communicator;

		public Chemist( AlchemyController controller, ICommunicator communicator )
		{
			_controller = controller;
			_communicator = communicator;
		}

		public void Cook()
		{
			_communicator.Display( "! to exit, + to set terminal element, > to add explicit element, # to recommend rule" );
			string em1 = null, em2 = null;
			while( true )
			{
				Rule testthisrule = _controller.RecommendNewRule( em1, em2 );
				if( testthisrule == Rule.EmptyRule )
					break;

				_communicator.Display( testthisrule == null ? "No elements recorded, please add." : testthisrule.ToString() );

				var reportback = _communicator.GetInput();
				if( reportback == null )
					continue;

				if( reportback.StartsWith( "#" ) )
				{
					var ems = reportback.TrimStart( '#' ).Split( ',' );
					if( ems.Length >= 2 )
					{
						em1 = ems[0].Trim();
						em2 = ems[1].Trim();
					}
					else
					{
						em1 = em2 = null;
					}
				}
				else
				{
					em1 = em2 = null;
					if( testthisrule == null || reportback.StartsWith( ">" ) )
						_controller.RegisterNewElement( reportback.TrimStart( '>' ) );
					else if( reportback.StartsWith( "+" ) )
					{
						_controller.FinalizeElement( reportback.TrimStart( '+' ) );
					}
					else if( reportback.StartsWith( "!" ) )
					{
						_communicator.Display( "Premature exit." );
						break;
					}
					else
					{
						testthisrule.SetResult( reportback );
						_controller.ReportChangedRule( testthisrule );
					}
				}
				_communicator.Display( "..." );
			}
			_communicator.Display( "Game is done" );
		}
	}
}
