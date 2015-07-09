using UnityEngine;
using System.Collections;
using System;
using Sinoze.Engine;
using Sinoze.Engine.Collections;

public class SinozeEngineTest : MonoBehaviour 
{	
	Siege siege1 = new Siege();
	Siege siege2 = new Siege();
	Siege siege3 = new Siege();
	Siege siege4 = new Siege();

	void Start () 
	{
		Debug.Log("SinozeEngineTest start");
		Logger.LogError("test error " + Time.frameCount);
		Assert.True(false, "test msg " + Time.frameCount, AssertLevel.Warning );
		Logger.Log("test caller : " + Diagnostic.GetCallerType().FullName);

//		siege1.Subscribe((s, msg) => { Logger.Log("s1 message received : " + msg); new Siege().Publish("test");}, SiegeSubscribeOption.SingleComsume);
//		siege2.Subscribe((s, msg) => { Logger.Log("s2 message received : " + msg); }, SiegeSubscribeOption.Persist);
//		siege3.Subscribe((s, msg) => { Logger.Log("s3 message received : " + msg); siege3.Dispose(); }, SiegeSubscribeOption.Persist);
	}

	void OnGUI()
	{
		//GUILayout.TextField(GetUserAgent.UserAgent_iOS);
		//GUILayout.Label(SiegeServer.Instance.ToString ());

		if(GUILayout.Button("Post"))
		{
			var msg = Time.frameCount;
			Logger.Log("broadcast msg : " + msg);
			Siege.Broadcast(msg);
		}

		if(GUILayout.Button("Listen"))
		{
			Logger.Log("subscribe");
			siege4.Subscribe((s, msg)=>{Logger.Log("subscription received : " + msg);}, SiegeSubscribeOption.Persist);
		}

		if(GUILayout.Button("Test Send Event"))
		{
			var e = new SinozeAnalyticsEvent("SinozeEvent");
			e.AddDimension("Play", "Yapid");
			SinozeAnalytics.RecordEvent(e);
		}
	}

	void OnDestroy()
	{	
		siege1.Dispose();
		siege2.Dispose();
		siege3.Dispose();
		siege4.Dispose();
	}
}
