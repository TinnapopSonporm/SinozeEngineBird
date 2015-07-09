using System.Collections;
using System.Collections.Generic;
using System;
using Sinoze.Engine;

[Module]
public sealed class SinozeAnalytics : AdaptableService
{
	#region API
	public static void RecordEvent(SinozeAnalyticsEvent e)
	{
		_instance.InvokeAdaptors("RecordEvent", e);
	}
	
	public static void RecordInAppPurchaseEvent(SinozeAnalyticsInAppPurchaseEvent e)
	{
		_instance.InvokeAdaptors("RecordInAppPurchaseEvent", e);
	}
	
	public static void RecordVirtualCurrencyEvent(SinozeAnalyticsVirtualCurrencyEvent e)
	{
		_instance.InvokeAdaptors("RecordVirtualCurrencyEvent", e);
	}
	#endregion
	
	private static SinozeAnalytics _instance;
	public SinozeAnalytics()
	{ 
		if(_instance != null)
			throw new InvalidOperationException("SinozeAnalytics already instanciated");
		_instance = this;
	}
}