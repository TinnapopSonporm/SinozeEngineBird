using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sinoze.Engine;

namespace Sinoze.Engine
{
	[Module]
	public sealed class Logger : AdaptableService, IUpdatable
	{
		private static Logger _instance;
		
		internal List<LogMeta> _logs = new List<LogMeta>();
		List<LogMeta> _logsThisFrame = new List<LogMeta>();
		List<string> _tags = new List<string>();
		
		public ReadOnlyCollection<LogMeta> Logs { get; private set; }
		public ReadOnlyCollection<LogMeta> LogsThisFrame { get; private set; }
		public ReadOnlyCollection<string> Tags { get; private set; }

		private int frameCount;

		public Logger()
		{
			if(_instance != null)
				throw new InvalidOperationException("Logger already instanciated");
			_instance = this;

			Logs = new ReadOnlyCollection<LogMeta>(_logs);
			LogsThisFrame = new ReadOnlyCollection<LogMeta>(_logsThisFrame);
			Tags = new ReadOnlyCollection<string>(_tags);
		}

		#region IUpdatable
		public void Update()
		{
			frameCount = EngineRoot.Instance.Environment.currentFrameIndex;
		}

		public void LateUpdate()
		{
			_logsThisFrame.Clear ();
		}
		#endregion

		private void Record(LogType logType, System.Diagnostics.StackTrace stackTrace, string callerClassName, object message, Exception exception, string[] tags)
		{
			// record a new log
			var log = new LogMeta()
			{
				logType = logType,
				stackTrace = stackTrace,
				callerClassName = callerClassName,
				dumpedStackTrace = Diagnostic.GetTraceString(stackTrace),
				timeStamp = DateTime.Now,
				frameIndex = frameCount,
				message = message,
				exception = exception,
				tags = tags,
			};
			_logs.Add(log);
			_logsThisFrame.Add(log);

			// also collect all unique tags
			if(tags != null)
			{
				for(int i=0;i<tags.Length;i++)
				{
					if(!_tags.Contains(tags[i]))
						_tags.Add(tags[i]);
				}
			}
		}

		#region Public API
		/// <summary>
		/// Log the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void Log(object message, params string[] tags)
		{
			var logger = _instance;
			string callerClassName;
			var trace = Diagnostic.GetStackTrace(1);
			var traceString = Diagnostic.GetTraceString(trace.GetFrame(0), out callerClassName);
			logger.Record(LogType.Log, trace, callerClassName, message, null, tags);

			_instance.InvokeAdaptors("Log", message + "\n" + traceString);
		}

		public static void LogError(object message, params string[] tags)
		{
			var logger = _instance;
			string callerClassName;
			var trace = Diagnostic.GetStackTrace(1);
			var traceString = Diagnostic.GetTraceString(trace.GetFrame(0), out callerClassName);
			logger.Record(LogType.Error, trace, callerClassName, message, null, tags);

			_instance.InvokeAdaptors("LogError", message + "\n" + traceString);
		}

		public static void LogException(System.Exception exception, params string[] tags)
		{
			var logger = _instance;
			string callerClassName;
			var trace = Diagnostic.GetStackTrace(1);
			Diagnostic.GetTraceString(trace.GetFrame(0), out callerClassName);
			logger.Record(LogType.Exception, trace, callerClassName, null, exception, tags);

			_instance.InvokeAdaptors("LogException", exception);
		}

		public static void LogWarning(object message, params string[] tags)
		{
			var logger = _instance;
			string callerClassName;
			var trace = Diagnostic.GetStackTrace(1);
			var traceString = Diagnostic.GetTraceString(trace.GetFrame(0), out callerClassName);
			logger.Record(LogType.Warning, trace, callerClassName, message, null, tags);

			_instance.InvokeAdaptors("LogWarning", message + "\n" + traceString);
		}
		#endregion

		internal static void LogAssert(object message, LogType logType, System.Exception exception, params string[] tags)
		{
			var logger = _instance;
			string callerClassName;
			var trace = Diagnostic.GetStackTrace(2);
			var traceString = Diagnostic.GetTraceString(trace.GetFrame(0), out callerClassName);
			logger.Record(logType, trace, callerClassName, message, null, tags);

			if(logType == LogType.Warning)
				_instance.InvokeAdaptors("LogWarning", message + "\n" + traceString);
			else if(logType == LogType.Error)
				_instance.InvokeAdaptors("LogError", message + "\n" + traceString);
			else if(logType == LogType.Exception)
				_instance.InvokeAdaptors("LogException", exception);
		}
	}

	public struct LogMeta
	{
		public LogType logType;
		public System.Diagnostics.StackTrace stackTrace;
		public string callerClassName;
		public string dumpedStackTrace;
		public DateTime timeStamp;
		public int frameIndex;
		public object message;
		public string[] tags;
		public Exception exception;
	}

	public enum LogType
	{
		Error, //	LogType used for Errors.
		Warning, //	LogType used for Warnings.
		Log, //	LogType used for regular log messages.
		Exception, //	LogType used for Exceptions.
	}
}