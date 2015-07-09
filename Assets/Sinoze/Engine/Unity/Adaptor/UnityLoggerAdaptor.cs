using UnityEngine;
using System.Collections;
using Sinoze.Engine;

namespace Sinoze.Engine.Unity
{
	[Module(Service = typeof(Logger))]
	public class UnityLoggerAdaptor : MonoBehaviour
	{
		Logger logger;

		void Start()
		{
			logger = Module.Find<Logger>();
		}
		
		#region Adaptor Methods
		
		public void Log (string message)
		{
			Debug.Log(message);
		}
		
		public void LogWarning (string message)
		{
			Debug.LogWarning(message);
		}
		
		public void LogError (string message)
		{
			Debug.LogError(message);
		}
		
		public void LogException (System.Exception exception)
		{
			Debug.LogException(exception);
		}

		#endregion
		
		Vector2 scroll;
		void OnGUI()
		{
			if(logger._logs.Count == 0)
				return;
			
			// display logs group by frameIndex
			
			scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(Screen.width));
			int currentFrame = logger._logs[0].frameIndex;
			string sum = "";
			foreach(var l in logger._logs)
			{
				if(l.frameIndex != currentFrame)
				{
					var endLine = "\n___________________________________________________";
					if(Mathf.Abs(l.frameIndex - currentFrame) > 10)
						endLine += endLine;
					GUILayout.Label("[FRAME " +currentFrame + "]" + sum + endLine);
					currentFrame = l.frameIndex;
					sum = "";
				}
				
				var msg = l.message == null ? "(no message)" : l.message.ToString ();
				var meta = "[" + l.logType + "] " + l.timeStamp.ToShortDateString() + " " + l.timeStamp.ToLongTimeString()  + "." + l.timeStamp.Millisecond + " (" + l.frameIndex + ")";
				var output = "";
				output += msg;
				output += "\n" + l.dumpedStackTrace;
				output += "      // " + meta;
				
				sum += "\n" + output;
				
			}
			
			if(!string.IsNullOrEmpty(sum))
			{
				GUILayout.Label("[FRAME " + currentFrame + "]" + sum + "\n___________________________________________________");
			}
			GUILayout.EndScrollView();
		}
	}
}