using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Alchemist;
using System.Xml.Serialization;

namespace UnitTests
{
	/// <summary>
	/// </summary>
	[TestClass]
	public class SerializationTests
	{
		[TestMethod]
		public void SerializeRoundtripToDisc()
		{
			const string filename = "testfiledata.xml";
			if( File.Exists( filename ) )
				File.Delete( filename );
			var streams = new StreamFactory( filename );
			var serializer = new RuleSetXmlSerializer();

			RuleSet secondrs;
			var rs = new RuleSet
			{
				FoundElements = new[]
				{
					new Element("fire"),
					new Element{Name= "water",TerminalValue=true}
				},
				Rules = new[]
				{
					new Rule( new []{"fire","fire"},"water")
				},
			};

			using( var stream = streams.CreateSerializingStream() )
				serializer.Serialize( stream, rs );
			using( var stream = streams.CreateDeserializingStream() )
				secondrs = (RuleSet) serializer.Deserialize( stream );

			Assert.AreEqual( rs, secondrs );

		}

		[TestMethod]
		public void SerializeRuleSet()
		{
			var rs = new RuleSet
			{
				FoundElements = new[] { new Element( "a" ) { Terminal = true }, new Element( "aasdf" ) },
				Rules = new[] { new Rule( new[] { "fire" }, "" ) }
			};

			TestSerializationRoundtrip( rs );
		}

		[TestMethod]
		public void SerializeRule()
		{
			var em = new Rule();
			TestSerializationRoundtrip( em );
		}

		[TestMethod]
		public void SerializationPersistTerminalForElement()
		{
			var rs = new Element( "fire" )
			{
				TerminalValue = true
			};

			var x = new XmlSerializer( rs.GetType() );
			var s = new StringWriter();
			x.Serialize( s, rs );

			var stringreader = new StringReader( s.ToString() );

			var result = (Element) x.Deserialize( stringreader );

			Assert.AreEqual( rs.TerminalValue, result.TerminalValue );
		}

		[TestMethod]
		public void SerializeElement()
		{
			const string em = "fire";
			TestSerializationRoundtrip( em );
		}

		[TestMethod]
		public void CanDeserializeWithoutFinalDataAttribute()
		{
			const string data = @"<?xml version=""1.0"" encoding=""utf-16""?><RuleSet xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><rule>
    <ingredient>fire</ingredient>
    <result />
  </rule>
  <element>a</element>
  <element>aasdf</element>
</RuleSet>";
			var x = new XmlSerializer( typeof( RuleSet ) );

			var stringreader = new StringReader( data );

			var result = (RuleSet) x.Deserialize( stringreader );
			Assert.AreEqual( 1, result.Rules.Count() );
			Assert.IsFalse( result.FoundElements.FirstOrDefault().TerminalValue.HasValue );

		}
		static void TestSerializationRoundtrip<T>( T entity )
		{
			var x = new XmlSerializer( entity.GetType() );
			var s = new StringWriter();
			x.Serialize( s, entity );

			var stringreader = new StringReader( s.ToString() );

			var result = (T) x.Deserialize( stringreader );

			Assert.AreEqual( entity, result );
			Assert.AreNotSame( entity, result );
		}
	}
}
