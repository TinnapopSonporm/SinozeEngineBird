using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public static class AttributeHelper
{
	public static IEnumerable<Type> GetTypesWithAttribute<T>()
	{
		var assembly = Assembly.GetExecutingAssembly();
		
		foreach(Type type in assembly.GetTypes()) 
		{
			if (Attribute.IsDefined(type, typeof(T)))
			{
				yield return type;
			}
		}
	}

	public static IEnumerable<Type> GetTypesImplementInterface<T>()
	{
		var assembly = Assembly.GetExecutingAssembly();
		
		foreach(Type type in assembly.GetTypes()) 
		{
			if(typeof(T).IsAssignableFrom(type) && typeof(T) != type)
			{
				yield return type;
			}
		}
	}
}
