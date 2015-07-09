using UnityEngine;
using Sinoze.Engine;
using System.IO;

public class Saver {

	public static void SaveTextureToFile (Texture2D texture, string filename, IMAGE_TYPE itype)
	{ 
		string path = Application.dataPath + "/"+ filename;
		string path2 = Application.persistentDataPath + "/"+ filename;

		if (itype == IMAGE_TYPE.JPG)
		{

		}
		else
		{

		}
		byte[] data;
		switch (itype)
		{

		case IMAGE_TYPE.JPG:
			data = texture.EncodeToJPG();
			Save(path, data);
			break;
		case IMAGE_TYPE.PNG:
			data = texture.EncodeToPNG();
			Save(path, data);
			Logger.Log ("Encoding to PNG");
			break;
		default:
			Logger.Log ("Incorrect IMAGE_TYPE.");
			break;
		}


		#if UNITY_EDITOR
		Logger.Log(path + " : WAS SAVE");
		#else
		Debug.Log (path2 + " : WAS SAVE");
		#endif

	}

	public static void Save(string path,byte[] data)
	{
		File.WriteAllBytes (path, data);
	}

	public enum IMAGE_TYPE
	{
		PNG, JPG
	}
}
