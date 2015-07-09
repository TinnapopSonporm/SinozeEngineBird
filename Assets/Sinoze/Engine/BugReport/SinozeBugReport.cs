using System.Collections;
using System.Collections.Generic;
using System;
using Sinoze.Engine;

[Module]
public sealed class SinozeBugReport : AdaptableService
{
	#region API
	public static void Submit(Exception exception, string context = null, SinozeBugSeverity severity = SinozeBugSeverity.Warning)
	{
		_instance.InvokeAdaptors("Submit", exception, context, severity);
	}
	public static void SubmitMessage(string name, string message, string context = null, SinozeBugSeverity severity = SinozeBugSeverity.Info)
	{
		_instance.InvokeAdaptors("SubmitMessage", name, message, context, severity);
	}
	#endregion

	private static SinozeBugReport _instance;
	public SinozeBugReport()
	{ 
		if(_instance != null)
			throw new InvalidOperationException("SinozeBugReport already instanciated");
		_instance = this;
	}
}