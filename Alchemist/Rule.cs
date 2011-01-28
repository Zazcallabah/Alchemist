using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Alchemist
{
	[SerializableAttribute]
	[XmlType]
	[XmlRoot]
	public class RuleSet : IEquatable<RuleSet>, INotifyPropertyChanged
	{
		public RuleSet()
		{
			Rules = new Rule[0];
			FoundElements = new Element[0];
		}

		public RuleSet DeepClone()
		{
			return new RuleSet
			{
				_foundElements = new List<Element>( _foundElements ),
				_rules = new List<Rule>( _rules )
			};
		}

		List<Rule> _rules;

		[XmlElement( "rule" )]
		public Rule[] Rules
		{
			get { return _rules.ToArray(); }
			set { _rules = new List<Rule>( value ); FirePropertyChanged( "Rules" ); }
		}

		List<Element> _foundElements;

		[XmlElement( "element" )]
		public Element[] FoundElements
		{
			get { return _foundElements.ToArray(); }
			set { _foundElements = new List<Element>( value ); FirePropertyChanged( "FoundElements" ); }
		}

		public bool Equals( RuleSet other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;

			return Rules.All( rules => other.Rules.Any( e => e == rules ) ) &&
			 FoundElements.All( element => other.FoundElements.Any( e => e == element ) );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != typeof( RuleSet ) )
				return false;
			return Equals( (RuleSet) obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ( ( Rules != null ? Rules.GetHashCode() : 0 ) * 397 ) ^
					( FoundElements != null ? FoundElements.GetHashCode() : 0 );
			}
		}

		public static bool operator ==( RuleSet left, RuleSet right )
		{
			return Equals( left, right );
		}

		public static bool operator !=( RuleSet left, RuleSet right )
		{
			return !Equals( left, right );
		}

		public Element AddNewElement( string elementName )
		{
			if( string.IsNullOrEmpty( elementName ) )
				throw new ArgumentException();
			if( _foundElements.Any( e => e.Name.Equals( elementName ) ) )
				throw new ArgumentException();

			var element = new Element( elementName );
			_foundElements.Add( element );
			FirePropertyChanged( "FoundElements" );

			return element;
		}

		public void AddTestedRule( Rule rule )
		{
			// swap rule elements with elements from list
			_rules.Add( rule );
			FirePropertyChanged( "Rules" );
		}

		void FirePropertyChanged( string propertyName )
		{
			if( null != PropertyChanged )
			{
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void SetTerminalElement( string elementname )
		{
			var element = _foundElements.FirstOrDefault( e => e.Name.Equals( elementname ) );
			if( element == null )
				return;

			element.TerminalValue = true;

			FirePropertyChanged( "FoundElements" );
		}
	}


	[SerializableAttribute]
	[XmlType]
	public class Element : IEquatable<Element>, IComparable<Element>
	{
		public Element()
			: this( "" ) { }

		public Element( string name )
		{
			Name = name;
		}

		[XmlText]
		public string Name
		{ get; set; }

		[XmlAttribute( "end" )]
		public virtual bool Terminal
		{
			get;
			set;
		}

		/// <remarks/>
		[XmlIgnoreAttribute]
		public bool? TerminalValue
		{
			get
			{
				return TerminalSpecified ? Terminal : new bool?();
			}
			set
			{
				if( value.HasValue )
				{
					TerminalSpecified = true;
					Terminal = value.Value;
				}
				else
				{
					TerminalSpecified = false;
				}
			}
		}

		/// <remarks/>
		[XmlIgnoreAttribute]
		public virtual bool TerminalSpecified
		{
			get;
			set;
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != typeof( Element ) )
				return false;
			return Equals( (Element) obj );
		}

		public int CompareTo( Element other )
		{
			return Name.CompareTo( other.Name );
		}

		public override string ToString()
		{
			return Name;
		}

		public bool Equals( Element other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return other.Name.Equals( Name );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return Name != null ? Name.GetHashCode() : 0;
			}
		}

		public static bool operator ==( Element left, Element right )
		{
			return Equals( left, right );
		}

		public static bool operator !=( Element left, Element right )
		{
			return !Equals( left, right );
		}
	}

	[SerializableAttribute]
	[XmlType]
	public class Rule : IEquatable<Rule>
	{
		public Rule( IEnumerable<string> ingredients )
			: this( ingredients, new string[0] ) { }

		public Rule( IEnumerable<string> ingredients, string result )
			: this( ingredients, new[] { result } ) { }

		public Rule( IEnumerable<string> ingredients, IEnumerable<string> result )
			: this( ingredients.Select( i => new Element( i ) ), result.Select( i => new Element( i ) ) )
		{ }

		public Rule()
			: this( new Element[0] ) { }

		public Rule( IEnumerable<Element> ingredients )
			: this( ingredients, new Element[0] ) { }

		public Rule( IEnumerable<Element> ingredients, Element result )
			: this( ingredients, new[] { result } ) { }

		public Rule( IEnumerable<Element> ingredients, IEnumerable<Element> result )
		{
			Ingredients = ingredients.Where( i => i != null ).ToArray();
			Result = result.Where( i => i != null ).ToArray();
		}

		Element[] _ingredients;
		int _ingredientCount;

		[XmlElement( "ingredient" )]
		public Element[] Ingredients
		{
			get { return _ingredients; }
			set
			{
				_ingredients = value;

				if( _ingredients != null )
				{
					_ingredientCount = _ingredients.Count();
					Array.Sort( _ingredients );
				}
				else
					_ingredientCount = 0;
			}
		}

		[XmlElement( "result" )]
		public Element[] Result { get; set; }

		private static readonly Rule Empty = new Rule();



		[XmlIgnore]
		public static Rule EmptyRule
		{
			get { return Empty; }
		}

		[XmlIgnore]
		public bool IsEmpty
		{
			get { return Result == null || Result.Count() == 0; }
		}

		public override string ToString()
		{
			string st = "";
			if( Ingredients != null )
				foreach( var el in Ingredients )
				{
					st += el;
					st += " + ";
				}

			return st + " -> ";
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != typeof( Rule ) )
				return false;
			return Equals( (Rule) obj );
		}

		public bool Equals( Rule other )
		{
			if( _ingredientCount != other._ingredientCount )
				return false;

			for( int i = 0; i < _ingredientCount; i++ )
			{
				if( !Ingredients[i].Equals( other.Ingredients[i] ) )
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ( ( Ingredients != null ? Ingredients.GetHashCode() : 0 ) * 397 ) ^ ( Result != null ? Result.GetHashCode() : 0 );
			}
		}

		public static bool operator ==( Rule left, Rule right )
		{
			return Equals( left, right );
		}

		public static bool operator !=( Rule left, Rule right )
		{
			return !Equals( left, right );
		}

		public void SetResult( string inputline )
		{
			if( Result != null && Result.Length != 0 )
				throw new ArgumentException();

			Result = inputline
				.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries )
				.Where( s => !string.IsNullOrEmpty( s ) )
				.Select( s => new Element( s.Trim() ) )
				.ToArray();
		}
	}
}