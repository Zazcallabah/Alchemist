using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Alchemist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	/// <summary>
	/// Summary description for NotificationTests
	/// </summary>
	[TestClass]
	public class NotificationTests
	{
		[TestMethod]
		[DeploymentItem( "xmldata\\data.xml" )]
		public void RuleSetOnlyNotifiesWhenStrictlyNeccessary()
		{
			var changedprops = new List<string>();
			var persister = new XmlPersister( new RuleSetXmlSerializer(), new StreamFactory( "data.xml" ), 2000 );
			var rs = persister.RecreateRuleSet();
			rs.PropertyChanged += ( s, e ) => changedprops.Add( e.PropertyName );

			var controller = new AlchemyController( rs );

			var rule = controller.RecommendNewRule();
			rule.Result = new[] { new Element( "alpha" ) };
			controller.ReportChangedRule( rule );

			Assert.AreEqual( 1, changedprops.Count( p => p.Equals( "FoundElements" ) ) );
			Assert.AreEqual( 1, changedprops.Count( p => p.Equals( "Rules" ) ) );
		}

		[TestMethod]
		public void RuleSetNotifiesOnPropertyChanged()
		{
			var changedprops = new List<string>();
			var r = new RuleSet();
			r.PropertyChanged += ( s, e ) => changedprops.Add( e.PropertyName );

			var controller = new AlchemyController( r );
			controller.RegisterNewElement( "fire" );
			var rule = controller.RecommendNewRule();
			rule.Result = new[] { new Element( "water" ) };

			controller.ReportChangedRule( rule );

			Assert.AreEqual( 2, changedprops.Count( p => p.Equals( "FoundElements" ) ) );
			Assert.AreEqual( 2, r.FoundElements.Count() );
			Assert.AreEqual( 1, changedprops.Count( p => p.Equals( "Rules" ) ) );
			Assert.AreEqual( 1, r.Rules.Count() );

		}

		[TestMethod]
		public void RuleSetNotificationSendsSelfAsObject()
		{
			RuleSet other = null;
			var r = new RuleSet();
			r.PropertyChanged += ( a, e ) => other = (RuleSet) a;
			r.Rules = new Rule[0];

			Assert.AreSame( r, other );

		}

		[TestMethod]
		public void RuleSetIsNotifyPropertyChanged()
		{
			var r = new RuleSet();
			Assert.IsInstanceOfType( r, typeof( INotifyPropertyChanged ) );
		}

	}
}
