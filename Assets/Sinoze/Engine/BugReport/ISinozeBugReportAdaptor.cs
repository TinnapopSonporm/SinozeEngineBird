using System;

namespace Sinoze.Engine
{
	public interface ISinozeBugReportAdaptor
	{
		void Submit(Exception exception, string context, SinozeBugSeverity severity);
		void SubmitMessage(string name, string message, string context, SinozeBugSeverity severity);
	}
}