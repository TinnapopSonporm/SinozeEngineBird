// v2.2.6
// Tested with Unity 5.1.1 iOS 8.3
// Tested with Unity 4.6.5 iOS 8.3

using Sinoze.Engine;

#if BUGSNAG
using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

[Module(Service = typeof(SinozeBugReport))]
public class BugSnagAdaptor : MonoBehaviour//, ISinozeBugReportAdaptor
{
	void Awake ()
	{
		var config = Config.Find<BugSnagConfig>();
		if(config == null)
		{
			EngineRoot.Instance.RemoveModule(this);
			return;
		}

		string apiKey = null;
		#if UNITY_ANDROID
		apiKey = config.android_apiKey;
		#elif UNITY_IOS
		apiKey = config.ios_apiKey;
		#endif
		if(string.IsNullOrEmpty(apiKey))
		{
			EngineRoot.Instance.RemoveModule(this);
			return;
		}

		Bugsnag.NativeBugsnag.Register(apiKey);
		Bugsnag.NativeBugsnag.SetReleaseStage(Debug.isDebugBuild ? "development" : "production");
		Bugsnag.NativeBugsnag.SetContext(Application.loadedLevelName);
		Bugsnag.NativeBugsnag.SetAutoNotify(true);

		#if UNITY_5
		Application.logMessageReceived += HandleLog;
		#else
		Application.RegisterLogCallback(HandleLog);
		#endif
	}

	public void Submit (Exception exception, string context, SinozeBugSeverity severity)
	{
		var stackTrace = !string.IsNullOrEmpty(exception.StackTrace) ? exception.StackTrace : Diagnostic.GetStackTrace(1).ToString ();
		Notify (exception.GetType ().ToString (), exception.Message, SeverityString(severity), context, stackTrace);
	}
	
	public void SubmitMessage(string name, string message, string context, SinozeBugSeverity severity)
	{
		var stackTrace = Diagnostic.GetStackTrace(1).ToString ();
		Notify (name, message, SeverityString(severity), context, stackTrace);
	}
	
	void OnLevelWasLoaded(int level) 
	{
		Bugsnag.NativeBugsnag.SetContext(Application.loadedLevelName);
	}
	
	// track unhandle exception
	// or explicit exception log
	void HandleLog (string logString, string stackTrace, UnityEngine.LogType type)
	{
		if(type != UnityEngine.LogType.Exception)
			return;

		string errorClass, errorMessage = "";
		var exceptionRegEx = new Regex(@"^(?<errorClass>\S+):\s*(?<message>.*)");
		var match = exceptionRegEx.Match(logString);
		if(match.Success) 
		{
			errorClass = match.Groups["errorClass"].Value;
			errorMessage = match.Groups["message"].Value.Trim();
		} 
		else 
		{
			errorClass = logString;
		}
		stackTrace = !string.IsNullOrEmpty(stackTrace) ? stackTrace : Diagnostic.GetStackTrace(1).ToString ();
		Notify (errorClass, errorMessage, SeverityString(SinozeBugSeverity.Error), "", stackTrace);
	}

	void Notify(string errorClass, string message, string severity, string context, string stackTrace) 
	{
		if (string.IsNullOrEmpty(errorClass)) errorClass = "Error";
		if (string.IsNullOrEmpty(message)) message = "";
	    if (string.IsNullOrEmpty(severity)) severity = "error";
	    if (string.IsNullOrEmpty(context)) context = "";
	    if (string.IsNullOrEmpty(stackTrace)) stackTrace = "";

		Bugsnag.NativeBugsnag.Notify(errorClass, message, severity, context, stackTrace);
	}

	string SeverityString(SinozeBugSeverity severity)
	{
		switch(severity)
		{
			case SinozeBugSeverity.Info : return "info";
			case SinozeBugSeverity.Warning : return "warning";
			case SinozeBugSeverity.Error : return "error";
			default : return null;
		}
	}
}
#endif

[Config(Name = "Bugsnag", Define = "BUGSNAG")]
public class BugSnagConfig
{
	public string ios_apiKey;
	public string android_apiKey;
}