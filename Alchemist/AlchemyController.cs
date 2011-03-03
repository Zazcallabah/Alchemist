using System;
using System.Linq;

namespace Alchemist
{
	public class AlchemyController
	{
		readonly RuleSet _rs;
		readonly ElementFactory _factory;

		public AlchemyController( RuleSet rs )
		{
			_rs = rs;
			_factory = new ElementFactory( rs );
			_persistedIndex = 0;
		}

		public AlchemyState State
		{
			get
			{
				if( _rs.FoundElements.Length == 0 )
					return AlchemyState.NotStarted;
				if( _rs.Rules.Length >= ( _rs.FoundElements.Length + 1 ) * _rs.FoundElements.Length / 2 )
					return AlchemyState.Finished;
				return AlchemyState.Started;
			}
		}

		public void RegisterNewElement( string element )
		{
			if( !string.IsNullOrEmpty( element ) )
			{
				_persistedIndex = 0;
				_factory.MakeElement( element );
			}
		}
		int _persistedIndex;
		public Rule RecommendNewRule()
		{
			if( _rs.FoundElements.Count() == 0 )
				return null;

			int elementCount = _rs.FoundElements.Count();
			for( int i = _persistedIndex; i < elementCount; i++ )
			{
				var em1 = _rs.FoundElements[i];
				if( em1.TerminalValue.HasValue && em1.Terminal )
					continue;

				for( int j = i; j < elementCount; j++ )
				{
					var em2 = _rs.FoundElements[j];
					if( em2.TerminalValue.HasValue && em2.Terminal )
						continue;

					var rule = NewRuleFromIngredients( em1, em2 );
					if( !_rs.Rules.Any( r => r.Equals( rule ) ) )
					{
						_persistedIndex = i;
						return rule;
					}
				}
			}


			return Rule.EmptyRule;
		}

		static Rule NewRuleFromIngredients( Element e1, Element e2 )
		{
			return new Rule( new[] { e1, e2 }, new Element[0] );
		}

		public void ReportChangedRule( Rule rule )
		{
			if( _rs.Rules.Any( r => r.Equals( rule ) ) )
				// update content or return?
				return;

			foreach( var element in rule.Result )
				RegisterNewElement( element.Name );

			_rs.AddTestedRule( rule );
		}

		public void FinalizeElement( string elementname )
		{
			_rs.SetTerminalElement( elementname );
		}

		public Rule RecommendNewRule( string s1, string s2 )
		{
			if( string.IsNullOrEmpty( s1 ) || string.IsNullOrEmpty( s2 ) )
				return RecommendNewRule();

			var e1 = new Element( s1 );
			var e2 = new Element( s2 );

			if( _rs.FoundElements.Contains( e1 ) && _rs.FoundElements.Contains( e2 ) )
			{
				var rule = NewRuleFromIngredients( e1, e2 );
				if( !_rs.Rules.Contains( rule ) )
					return rule;
			}

			return RecommendNewRule();
		}

		public void ForeachNonterminalElement( Action<Element> @do )
		{
			foreach( var e in _rs.FoundElements.Where( e => !e.TerminalSpecified || ( e.TerminalSpecified && !e.Terminal ) ) )
				@do( e );
		}
	}

	public enum AlchemyState
	{
		NotStarted,
		Started,
		Finished
	}
}