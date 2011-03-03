using Alchemist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// Summary description for ElementFactory
	/// </summary>
	[TestClass]
	public class ElementFactoryTests
	{
		[TestMethod]
		public void ElementFactoryCreatesCachedElementsFromRuleSet()
		{
			const string elementname = "aoeu";
			var rs = new RuleSet { FoundElements = new[] { new Element( elementname ) } };

			var factory = new ElementFactory( rs );

			var otherelement = factory.MakeElement( elementname );

			Assert.AreEqual( elementname, otherelement.Name );
		}

		[TestMethod]
		public void ElementFactoryCanCreateAndCacheNewElement()
		{
			const string elementname = "aoeu";
			var rs = new RuleSet();
			var factory = new ElementFactory( rs );

			var element = factory.MakeElement( elementname );
			var otherelement = factory.MakeElement( elementname );

			Assert.IsNotNull( element );
			Assert.AreSame( element, otherelement );
		}


		[TestMethod]
		public void FactoryKeepsTrackOfExistingElementsAndReportNonexistenElements()
		{
			var rs = new RuleSet();
			var factory = new ElementFactory( rs );

			Assert.IsFalse( factory.ExistingElement( "aou" ) );
		}

		[TestMethod]
		public void FactoryKeepsTrackOfExistingElementsAndReportExistingElements()
		{
			var rs = new RuleSet();
			var factory = new ElementFactory( rs );
			factory.MakeElement( "fire" );

			Assert.IsTrue( factory.ExistingElement( "fire" ) );
		}

		[TestMethod]
		public void CreatedElementTrimsWhitespaceInName()
		{
			var rs = new RuleSet();
			var factory = new ElementFactory( rs );
			factory.MakeElement( "   fire   " );

			Assert.IsTrue( factory.ExistingElement( "fire" ) );
		}
	}
}
