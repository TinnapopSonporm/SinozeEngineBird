using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System;
using Sinoze.Engine;

public static class XmlHelper
{
	public static string Serialize(Type type, object obj)	
	{
		string result = null;
		try
		{
			XmlSerializer  xmlserializer = new XmlSerializer(type);
			StringWriter str  = new StringWriter();
			XmlWriter writer = XmlWriter.Create(str);
			xmlserializer.Serialize(writer, obj);
			result = str.ToString();
		}
		catch(Exception e)
		{
			UnityEngine.Debug.Log(e);
			result = null;
		}
		return result;
	}

	public static string Serialize<T>(T obj)	
	{
		return Serialize(typeof(T), obj);
	}
	
	public static object Deserialize(Type type, string xml)
	{
		object result = null;
		try
		{
			XmlSerializer  xmlserializer = new XmlSerializer(type);
			StringReader str = new StringReader(xml);
			XmlReader reader = XmlReader.Create(str);
			result = xmlserializer.Deserialize(reader);
		}
		catch(Exception e)
		{
			UnityEngine.Debug.Log(e);
			result = Activator.CreateInstance(type);
		}
		return result;
	}

	public static T Deserialize<T>(string xml)
	{
		return (T)Deserialize(typeof(T), xml);
	}
}
