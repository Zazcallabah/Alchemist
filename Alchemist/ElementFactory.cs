using System.Linq;

namespace Alchemist
{
	public class ElementFactory
	{
		readonly RuleSet _ruleSet;

		public ElementFactory( RuleSet ruleSet )
		{
			_ruleSet = ruleSet;
		}

		public Element MakeElement( string elementname )
		{
			return _ruleSet.FoundElements.FirstOrDefault( element => element.Name.Equals( elementname ) ) ??
				_ruleSet.AddNewElement( elementname );
		}

		public bool ExistingElement( string elementname )
		{
			return _ruleSet.FoundElements.Any( e => e.Name.Equals( elementname ) );
		}
	}
}