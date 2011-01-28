using System;

namespace Alchemist
{
	public class Communicator : ICommunicator
	{
		public void Display( string data )
		{
			Console.WriteLine( data );
		}

		public string GetInput()
		{
			return Console.ReadLine();
		}
	}
}