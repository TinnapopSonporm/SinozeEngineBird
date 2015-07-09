// v2.0.0.1
// http://docs.aws.amazon.com/mobile/sdkforunity/developerguide/analytics.html

using Sinoze.Engine;

#if AWS_ANALYTICS
using UnityEngine;
using System.Collections;
using Amazon.MobileAnalytics.MobileAnalyticsManager;
using Amazon.CognitoIdentity;
using Amazon.Util.Internal;
using Sinoze.Engine.Unity;

[Module(Service = typeof(SinozeAnalytics))]
public class AWSAnalyticsAdaptor : MonoBehaviour//, ISinozeAnalyticsAdaptor
{
	private MobileAnalyticsManager analyticsManager;
	private CognitoAWSCredentials _credentials;

	void Start()
	{
		var awsConfig = Config.Find<AwsConfig>();
		var awsAnalyticsConfig = Config.Find<AwsAnalyticsConfig>();
		
#if UNITY_EDITOR
		/// This is just to spoof the application to think that its running on iOS platform
		AmazonHookedPlatformInfo.Instance.Platform = "iPhoneOS";
		AmazonHookedPlatformInfo.Instance.Model = "iPhone";
		AmazonHookedPlatformInfo.Instance.Make = "Apple";
		AmazonHookedPlatformInfo.Instance.Locale = "en_US";
		AmazonHookedPlatformInfo.Instance.PlatformVersion = "8.1.2";
		
		AmazonHookedPlatformInfo.Instance.Title = "YourApp";
		AmazonHookedPlatformInfo.Instance.VersionName = "v1.0";
		AmazonHookedPlatformInfo.Instance.VersionCode = "1.0";
		AmazonHookedPlatformInfo.Instance.PackageName = "com.yourcompany.yourapp";
#endif

		_credentials = new CognitoAWSCredentials(awsConfig.cognitoID, Amazon.RegionEndpoint.USEast1);
		analyticsManager = MobileAnalyticsManager.GetOrCreateInstance(_credentials, Amazon.RegionEndpoint.USEast1, awsAnalyticsConfig.appId);
	}

	public void RecordEvent (SinozeAnalyticsEvent e)
	{
		var c = new CustomEvent(e.Name);

		if(e._stringMetas != null)
			foreach(var a in e._stringMetas)
				c.AddAttribute(a.Key, a.Value);
		if(e._numberMetas != null)
			foreach(var m in e._numberMetas)
				c.AddMetric(m.Key, m.Value);
		
		analyticsManager.RecordEvent(c);
	}

	public void RecordInAppPurchaseEvent(SinozeAnalyticsInAppPurchaseEvent e)
	{
		throw new System.NotImplementedException();
	}

	public void RecordVirtualCurrencyEvent(SinozeAnalyticsVirtualCurrencyEvent e)
	{
		throw new System.NotImplementedException();
	}

	void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			analyticsManager.ResumeSession();
		}
		else
		{
			analyticsManager.PauseSession ();
		}
	}
}
#endif


[Config(Name = "AWS Analytics", 
        Define = "AWS_ANALYTICS", 
        Dependency = typeof(AwsConfig))]
public class AwsAnalyticsConfig
{
	public string appId;
}
