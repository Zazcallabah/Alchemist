namespace Alchemist
{
	public interface ICommand
	{
		bool AppliesTo( string input );
		Do Run( string input, AlchemyController controller, ICommunicator communicator );
		string ToString();
		int Priority { get; }
	}
}