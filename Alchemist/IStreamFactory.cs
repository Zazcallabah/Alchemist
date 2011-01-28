using System.IO;

namespace Alchemist
{
	public interface IStreamFactory
	{
		Stream CreateSerializingStream();

		Stream CreateDeserializingStream();
	}
}