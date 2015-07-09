// V3
// Tested with Unity5.1.1 iOS 8.3

using UnityEngine;
using System.Collections;
using Sinoze.Engine;
using Sinoze.Engine.Unity;

// requirement : modify GoogleAnalyticsV3 from Awake() to Start()
#if GOOGLEANALYTICS
[Module(Service = typeof(SinozeAnalytics))]
public class GoogleAnalyticsAdaptor 
{
	public GoogleAnalyticsV3 ga;

	public GoogleAnalyticsAdaptor()
	{
		var config = Config.Find<GoogleAnalyticsConfig>();
		if(config == null)
		{
			EngineRoot.Instance.RemoveModule(this);
			return;
		}

		ga = UnityRoot.Instance.gameObject.AddComponent<GoogleAnalyticsV3>();

		ga.IOSTrackingCode = config.IOSTrackingCode;
		ga.androidTrackingCode = config.androidTrackingCode;
		ga.otherTrackingCode = config.otherTrackingCode;
		ga.productName = config.productName;
		ga.bundleIdentifier = config.bundleIdentifier;
		ga.bundleVersion = config.bundleVersion;
		ga.sendLaunchEvent = config.sendLaunchEvent;
		ga.logLevel = GoogleAnalyticsV3.DebugMode.VERBOSE;
		ga.sendLaunchEvent = true;

		ga.StartSession();
	}
	

	public void RecordEvent(SinozeAnalyticsEvent e)
	{
		var s = new EventHitBuilder();

		// category + action + label + value
		s.SetEventCategory(e.Category);
		s.SetEventAction(e.Action);
		s.SetEventLabel(e.Label);
		if(e.Value is int || e.Value is long)
			s.SetEventValue((long)e.Value);

		// custom dimensions
		if(e._dimensions != null)
		{
			foreach(var d in e._dimensions)
			{
				int dimensionIndex;
				if(int.TryParse(d.Key, out dimensionIndex))
				{
					s.SetCustomDimension(dimensionIndex, d.Value);
				}
			}
		}

		// custom matrics
		if(e._matrics != null)
		{
			foreach(var m in e._matrics)
			{
				int metricIndex;
				if(int.TryParse(m.Key, out metricIndex))
				{
					s.SetCustomMetric(metricIndex, m.Value.ToString ());
				}
			}
		}

		ga.LogEvent(s);
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

[Config(Name = "Google Analytics", Define = "GOOGLEANALYTICS")]
public class GoogleAnalyticsConfig
{
	public string IOSTrackingCode;
	public string androidTrackingCode;
	public string otherTrackingCode;
	public string productName;
	public string bundleIdentifier;
	public string bundleVersion;
	public bool sendLaunchEvent;
}
