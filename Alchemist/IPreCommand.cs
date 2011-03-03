namespace Alchemist
{
	public interface IPreCommand
	{
		Do Run( AlchemyController controller, ICommunicator communicator );
		int Priority { get; }
	}
}