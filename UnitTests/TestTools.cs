﻿using System;
using Alchemist;

namespace UnitTests
{
	public static class TestTools
	{
		public static Rule[] GenerateRules( int count )
		{
			return GenerateList( () => new Rule() { Ingredients = GenerateElements( 2 ), Result = GenerateElements( 1 ) }, count );
		}

		public static Element[] GenerateElements( int count )
		{
			var r = new Random();
			return GenerateList( () => new Element( r.Next().ToString() ), count );
		}

		public static T[] GenerateList<T>( Func<T> createRandom, int count )
		{
			var tRet = new T[count];
			for( var i = 0; i < count; i++ )
			{
				tRet[i] = createRandom();
			}
			return tRet;
		}

		public static RuleSet GetFromXml( string filename )
		{
			var serializer = new RuleSetXmlSerializer();
			var factory = new StreamFactory( filename );
			var persister = new XmlPersister( serializer, factory, 2000 );
			return persister.RecreateRuleSet();
		}
	}
}