using UnityEngine;
using System.Collections;

//[Sinoze.Engine.Module(EditorModeOnly = true)]
public class TestAnalytics : MonoBehaviour 
{
	void Start () {
	
	}

	void Update () {
	
	}

	void OnGUI()
	{
		if(GUILayout.Button("Log GameSession:Start"))
		{
			var e = new SinozeAnalyticsEvent("GameSession", "Start", "Let it go");
			e.AddDimension("1", "Expert");
			e.AddDimension("2", "Arcade");
			e.AddMatric("1", 120);
			SinozeAnalytics.RecordEvent(e);
		}
		
		if(GUILayout.Button("Log GameSession:End"))
		{
			var e = new SinozeAnalyticsEvent("GameSession", "End", "Let it go");
			e.AddDimension("1", "Expert");
			e.AddDimension("2", "Arcade");
			SinozeAnalytics.RecordEvent(e);
		}
	}
}
