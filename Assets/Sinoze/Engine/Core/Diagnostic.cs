using System.Diagnostics;

namespace Sinoze.Engine
{
	/// <summary>
	/// Helper class around System.Diagnostics.StackTrace
	/// </summary>
	public static class Diagnostic 
	{
		/// <summary>
		/// Gets the stack trace.
		/// </summary>
		/// <returns>The stack trace.</returns>
		/// <param name="framesToSkip">Frames to skip.</param>
		public static StackTrace GetStackTrace(int framesToSkip)
		{
			// always plus 1 frame to skip this method frame
			return new StackTrace(framesToSkip + 1, true);
		}

		/// <summary>
		/// Dumps the stack trace.
		/// </summary>
		/// <returns>The stack trace.</returns>
		public static string GetTraceString()
		{
			return GetTraceString(GetStackTrace(0));
		}

		/// <summary>
		/// Dumps the stack trace.
		/// </summary>
		/// <returns>The stack trace.</returns>
		/// <param name="trace">Trace.</param>
		public static string GetTraceString(StackTrace trace)
		{
			var sb = new System.Text.StringBuilder();
			for(int i=0;i<trace.FrameCount;i++)
			{
				var frame = trace.GetFrame(i);
				sb.AppendLine(GetTraceString(frame));
			}
			return sb.ToString ();
		}

		/// <summary>
		/// Gets the trace string.
		/// </summary>
		/// <returns>The trace string.</returns>
		/// <param name="frame">Frame.</param>
		public static string GetTraceString(StackFrame frame)
		{
			string callerClassName;
			return GetTraceString(frame, out callerClassName);
		}

		/// <summary>
		/// Gets the trace string.
		/// </summary>
		/// <returns>The trace string.</returns>
		/// <param name="frame">Frame.</param>
		/// <param name="callerClassName">Caller class name.</param>
		public static string GetTraceString(StackFrame frame, out string callerClassName)
		{
			var fileName = frame.GetFileName();
//			if(!string.IsNullOrEmpty(fileName))
//			{
//				var indexToTrim = fileName.IndexOf("/Assets"); // hardcode for Unity folder layout
//				fileName = fileName.Remove(0, indexToTrim + 7);
//			}
			var line = frame.GetFileLineNumber();
			var method = frame.GetMethod();
			var methodName = method.Name;
			callerClassName = method.ReflectedType.Name;
			return "      at " + callerClassName + "." + methodName + "() (in " + fileName + ":" + line + ")";
		}

		public static System.Type GetCallerType()
		{
			return GetStackTrace(1).GetFrame(0).GetMethod().ReflectedType;
		}
	}
}