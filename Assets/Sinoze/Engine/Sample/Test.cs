using UnityEngine;
using System.Collections;
using Sinoze.Engine;

//[Module]
public class Test : MonoBehaviour 
{
	public static void AutoResize(int screenWidth, int screenHeight)
	{
		Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
	}
	
	void OnGUI()
	{
		AutoResize(480, 320);
		
		if(GUILayout.Button("Submit Exception"))
		{
			SinozeBugReport.Submit(new System.Exception("test submit exception"), "Context1");
		}
		if(GUILayout.Button("Submit Message"))
		{
			SinozeBugReport.SubmitMessage("name", "test submit message", "Context2");
		}
		if(GUILayout.Button("Throw Exception"))
		{
			throw new System.Exception("test throw exception");
		}
		if(GUILayout.Button("Log Exception"))
		{
			Debug.LogException(new System.Exception("test log exception"));
		}
		if(GUILayout.Button("Crash"))
		{
			Crash(1);
		}
	}

	public void Crash(int i)
	{
		Crash(i++);
	}
}
