using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System;

namespace Sinoze.Engine.Editor
{
	public partial class GameSetupDialog
	{
		const int fileVersion = 1;
		string filePath = Application.dataPath + "/../gamestartcontext";

		void Save()
		{
			var file = File.Create(filePath);
			var binwrite = new BinaryWriter(file);
			var strwrite  = new StringWriter();
			var xmlwrite = XmlWriter.Create(strwrite);
			
			binwrite.Write(fileVersion);
			var xmlserializer = new XmlSerializer(engineStartParams.type);
			xmlserializer.Serialize(xmlwrite, engineStartParams.GetResult());
			binwrite.Write(strwrite.ToString ());
			strwrite.Close ();
			xmlwrite.Close ();

			binwrite.Write(customGameStartParams.Count);
			foreach(var d in customGameStartParams)
			{
				binwrite.Write(d.Value.enabled);
				binwrite.Write(d.Key.AssemblyQualifiedName);

				strwrite = new StringWriter();
				xmlwrite = XmlWriter.Create(strwrite);
				var xmlserializer2 = new XmlSerializer(d.Key);
				xmlserializer2.Serialize(xmlwrite, d.Value.GetResult());
				binwrite.Write(strwrite.ToString ());
				strwrite.Close();
				xmlwrite.Close ();
			}
			strwrite.Close ();
			binwrite.Close();
			xmlwrite.Close ();
		}
		
		void Load()
		{
			if(!File.Exists(filePath))
				return;

			Stream file = null;
			BinaryReader binread = null;

			try
			{
				file = File.OpenRead(filePath);
				binread = new BinaryReader(file);
				var version = binread.ReadInt32();
				if(version == 1)
					LoadV1(binread);
			}
			catch(Exception e)
			{
				if(binread != null)
				{
					binread.Close();
					binread = null;
				}
				if(file != null)
				{
					file.Close();
					file.Dispose();
				}
				UnityEngine.Debug.LogException(e);
			}
		}

		void LoadV1(BinaryReader binread)
		{
			var strread = new StringReader(binread.ReadString());
			var xmlread = XmlReader.Create(strread);
			var xmlserializer = new XmlSerializer(engineStartParams.type);
			engineStartParams.Load(xmlserializer.Deserialize(xmlread));
			strread.Close ();
			xmlread.Close ();
			
			int count = binread.ReadInt32();
			for(int i=0;i<count;i++)
			{
				var enabled = binread.ReadBoolean();
				var typestr = binread.ReadString();
				var type = Type.GetType (typestr);
				if(type == null) 
				{
					binread.ReadString(); // can't find type, so just read and throw away
					continue;
				}
				var xmlserializer2 = new XmlSerializer(type);
				strread = new StringReader(binread.ReadString());
				xmlread = XmlReader.Create(strread);
				customGameStartParams[type].Load(xmlserializer2.Deserialize(xmlread));
				strread.Close ();
				xmlread.Close ();
				customGameStartParams[type].enabled = enabled;
			}
			binread.Close();
			xmlread.Close();
		}
	}
}