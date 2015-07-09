// v2.1.0
// Tested with Unity 5.1.1 iOS 8.3
// Tested with Unity 4.6.5 iOS 8.3
using Sinoze.Engine;

#if GAMEANALYTICS
using System;
using Sinoze.Engine.Unity;
using GameAnalyticsSDK;

[Module(Service = typeof(SinozeAnalytics))]
[Module(Service = typeof(SinozeBugReport))]
public class GameAnalyticsAdaptor
{
	public GameAnalyticsAdaptor()
	{
		var config = Config.Find<GameAnalyticsConfig>();
		if(config == null)
		{
			EngineRoot.Instance.RemoveModule(this);
			return;
		}

		string gameKey = null;
		string secretKey = null;

		#if UNITY_ANDROID
		gameKey = config.android_gameKey;
		secretKey = config.android_secretKey;
		#elif UNITY_IOS
		gameKey = config.ios_gameKey;
		secretKey = config.ios_secretKey;
		#endif

		if(string.IsNullOrEmpty(gameKey) || string.IsNullOrEmpty (secretKey))
		{
			EngineRoot.Instance.RemoveModule(this);
			return;
		}

		GameAnalytics.SettingsGA.UpdateGameKey(0, gameKey);
		GameAnalytics.SettingsGA.UpdateSecretKey(0, secretKey);
		GameAnalytics.SettingsGA.Build = new string[] { "0.1" }; 
		GameAnalytics.SettingsGA.VerboseLogBuild  = true;
		
		UnityRoot.Instance.gameObject.AddComponent<GameAnalytics>();
		UnityRoot.Instance.gameObject.AddComponent<GA_SpecialEvents>();
	}

	#region Analytics
	public void RecordEvent (SinozeAnalyticsEvent e)
	{
		float? val = null;
		if(e.Value != null)
		{
			float tmp;
			if(float.TryParse(e.Value.ToString(), out tmp))
				val = tmp;
		}

		if(val.HasValue)
			GameAnalytics.NewDesignEvent(e.Name, val.Value);
		else
			GameAnalytics.NewDesignEvent(e.Name);
	}
	
	public void RecordInAppPurchaseEvent(SinozeAnalyticsInAppPurchaseEvent e)
	{
		throw new System.NotImplementedException();
	}
	
	public void RecordVirtualCurrencyEvent(SinozeAnalyticsVirtualCurrencyEvent e)
	{
		throw new System.NotImplementedException();
	}
	#endregion

	#region Bug Report

	public void Submit(Exception exception, string context = null, SinozeBugSeverity severity = SinozeBugSeverity.Warning)
	{
		var s = new System.Text.StringBuilder();

		s.AppendLine("[Exception]");
		s.AppendLine(exception.GetType().Name);

		if(!string.IsNullOrEmpty(exception.Message))
		{
			s.AppendLine("[Exception Message]");
			s.AppendLine(exception.Message);
		}

		if(!string.IsNullOrEmpty(context))
		{
			s.AppendLine("[Context]");
			s.AppendLine(context);
		}

		GameAnalytics.NewErrorEvent(Severity(severity), s.ToString());
	}

	public void SubmitMessage(string name, string message, string context = null, SinozeBugSeverity severity = SinozeBugSeverity.Info)
	{
		var s = new System.Text.StringBuilder();
		
		s.AppendLine("[Name]");
		s.AppendLine(name);
		
		if(!string.IsNullOrEmpty(message))
		{
			s.AppendLine("[Message]");
			s.AppendLine(message);
		}
		
		if(!string.IsNullOrEmpty(context))
		{
			s.AppendLine("[Context]");
			s.AppendLine(context);
		}
		
		GameAnalytics.NewErrorEvent(Severity(severity), s.ToString());
	}


	private GA_Error.GAErrorSeverity Severity(SinozeBugSeverity severity)
	{
		switch(severity)
		{
			case SinozeBugSeverity.Error : return GA_Error.GAErrorSeverity.GAErrorSeverityError;
			case SinozeBugSeverity.Warning : return GA_Error.GAErrorSeverity.GAErrorSeverityWarning;
			case SinozeBugSeverity.Info : return GA_Error.GAErrorSeverity.GAErrorSeverityInfo;
			default : return GA_Error.GAErrorSeverity.GAErrorSeverityError;
		}
	}

	#endregion
}
#endif

[Config(Name = "Game Analytics", Define = "GAMEANALYTICS")]
public class GameAnalyticsConfig
{
	public string ios_gameKey;
	public string ios_secretKey;
	public string android_gameKey;
	public string android_secretKey;
}
