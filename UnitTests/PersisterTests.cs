using System.IO;
using System.Text;
using System.Xml.Serialization;
using Alchemist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// Summary description for PersisterTests
	/// </summary>
	[TestClass]
	public class PersisterTests
	{
		[TestMethod]
		public void PersisterAttemptsToSerialize()
		{
			var serializer = new TestXmlSerializer();
			var stream = new TestStreamFactory { CurrentStream = new MemoryStream() };
			var persister = new XmlPersister( serializer, stream );
			var rs = new RuleSet();
			persister.RegisterRuleSet( rs );
			var controller = new AlchemyController( rs );
			controller.RegisterNewElement( "aoeu" );

			Assert.AreEqual( 1, serializer.SerializeCount );
		}

		[TestMethod]
		public void PersisterActuallySerializesToXml()
		{
			const string elementname = "verylongnamenotlikelytooccurnaturallyinxml_SSohahaa222";
			var serializer = new RuleSetXmlSerializer();
			var streamcontents = new byte[2048];
			var stream = new TestStreamFactory { CurrentStream = new MemoryStream( streamcontents ) };

			var persister = new XmlPersister( serializer, stream );
			var rs = new RuleSet();
			persister.RegisterRuleSet( rs );
			var controller = new AlchemyController( rs );
			controller.RegisterNewElement( elementname );

			Assert.IsTrue( Encoding.UTF8.GetString( streamcontents ).Contains( elementname ) );
		}

		[TestMethod]
		public void PersisterCanInitializeRuleSetFromSource()
		{
			var serializer = new RuleSetXmlSerializer();
			var rs = new RuleSet
				{
					FoundElements =
					new[] { new Element( "fire" ), new Element( "water" ) },
					Rules = new[]
					{
						new Rule( new [] {new Element( "fire" ),new Element( "fire" )})
					}
				};

			var xml = CreateSerializedStreamFromRuleSet( rs );

			var streamcontents = new byte[2048];
			var stream = new TestStreamFactory { CurrentStream = new MemoryStream( streamcontents ) };
			stream.CurrentStream = xml;
			stream.CurrentStream.Position = 0;
			var persister = new XmlPersister( serializer, stream );
			var other = persister.RecreateRuleSet();

			Assert.AreEqual( rs, other );
		}

		static MemoryStream CreateSerializedStreamFromRuleSet( RuleSet rs )
		{
			var serializer = new XmlSerializer( typeof( RuleSet ) );
			var stream = new MemoryStream();
			serializer.Serialize( stream, rs );
			return stream;
		}
	}

	public class TestStreamFactory : IStreamFactory
	{
		public MemoryStream CurrentStream { get; set; }
		public Stream CreateSerializingStream()
		{
			return CurrentStream;
		}

		public Stream CreateDeserializingStream()
		{
			return CurrentStream;
		}
	}

	public class TestXmlSerializer : IXmlSerializer
	{
		public int SerializeCount;
		public void Serialize( Stream s, object o )
		{
			SerializeCount++;
		}

		public int DeserializeCount;
		public object DeserializedObject { get; set; }
		public object Deserialize( Stream s )
		{
			DeserializeCount++;
			return DeserializedObject;
		}

	}
}
