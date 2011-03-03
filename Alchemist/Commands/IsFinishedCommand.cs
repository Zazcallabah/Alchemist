using System;

namespace Alchemist.Commands
{
	public class IsFinishedCommand : IPreCommand
	{
		public Do Run( AlchemyController controller, ICommunicator communicator )
		{
			return controller.State == AlchemyState.Finished ? Do.Exit : Do.KeepProcessing;
		}

		public int Priority
		{
			get { return 100; }
		}
	}
}
