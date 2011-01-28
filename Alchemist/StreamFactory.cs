using System.IO;

namespace Alchemist
{
	public class StreamFactory : IStreamFactory
	{
		readonly string _filename;

		public StreamFactory( string filename )
		{
			_filename = filename;
		}

		public Stream CreateSerializingStream()
		{
			return new BufferedStream( File.OpenWrite( _filename ) );
		}

		public Stream CreateDeserializingStream()
		{
			return new BufferedStream( File.OpenRead( _filename ) );
		}
	}
}