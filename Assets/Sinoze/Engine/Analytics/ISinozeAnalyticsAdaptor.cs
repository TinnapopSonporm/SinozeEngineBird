using System;

namespace Sinoze.Engine
{
	public interface ISinozeAnalyticsAdaptor
	{
		void RecordEvent(SinozeAnalyticsEvent e);
	}
}