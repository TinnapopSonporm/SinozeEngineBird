// (built-in)
// Tested with Unity 5.1.1 iOS 8.3

// v1.9.3
// Tested with Unity 4.6.5 iOS 8.3
using Sinoze.Engine;

#if UNITY_ANALYTICS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5
using UnityAnalytics = UnityEngine.Analytics.Analytics;
#else
using UnityAnalytics =  UnityEngine.Cloud.Analytics.UnityAnalytics;
#endif

[Module(Service = typeof(SinozeAnalytics))]
public class UnityAnalyticsAdaptor 
{
	public UnityAnalyticsAdaptor()	
	{
		var config = Config.Find<UnityAnalyticsConfig>();
		if(config == null)
		{
			EngineRoot.Instance.RemoveModule(this);
			return;
		}

		// ** For Unity5, have to manually set the Cloud Project Id in player settings
		#if !UNITY_5
		string projectId = null;
		#if UNITY_ANDROID
		projectId = config.android_projectId;
		#elif UNITY_IOS
		projectId = config.ios_projectId;
		#endif

		// no project id
		if(string.IsNullOrEmpty(projectId))
		{
			EngineRoot.Instance.RemoveModule(this);
			return;
		}

		UnityAnalytics.StartSDK(projectId);
		#endif
	}

	public void RecordEvent(SinozeAnalyticsEvent e)
	{
		var dict = new Dictionary<string, object>();

		if(e._dimensions != null)
			foreach(var a in e._dimensions)
				dict.Add(a.Key, a.Value);
		if(e._matrics != null)
			foreach(var m in e._matrics)
				dict.Add(m.Key, m.Value);

		UnityAnalytics.CustomEvent(e.Name, dict);
	}

	public void RecordInAppPurchaseEvent(SinozeAnalyticsInAppPurchaseEvent e)
	{
		throw new System.NotImplementedException();
	}
	
	public void RecordVirtualCurrencyEvent(SinozeAnalyticsVirtualCurrencyEvent e)
	{
		throw new System.NotImplementedException();
	}
}
#endif

[Config(Name = "Unity Analytics", Define = "UNITY_ANALYTICS")]
public class UnityAnalyticsConfig
{
	// ** For Unity5, have to manually set the Cloud Project Id in player settings
	#if !UNITY_5
	public string ios_projectId;
	public string android_projectId;
	#endif
}