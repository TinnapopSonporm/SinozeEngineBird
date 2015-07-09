using System.Collections;
using System.Collections.Generic;

public class SinozeAnalyticsEvent
{
	internal Dictionary<string,string> _dimensions;
	internal Dictionary<string,double> _matrics;
	public string Name { get; private set; }
	public object Value { get; private set; }

	// special case for google analytics
	public string Category { get; private set; }
	public string Action { get; private set; }
	public string Label { get; private set; }

	public SinozeAnalyticsEvent(string name, object value = null)
	{
		this.Name = name;
		this.Value = value;
	}

	// special case for google analytics
	public SinozeAnalyticsEvent(string category, string action, string label = null, long value = 1)
	{
		this.Name = "Category=" + category + ";Action=" + action + ";Label=" + label;
		this.Category = category;
		this.Action = action;
		this.Label = label;
		this.Value = value;
	}

	public void AddDimension(string attributeNameOrIndex, string attributeValue)
	{
		if(_dimensions == null)
			_dimensions = new Dictionary<string, string>();

		_dimensions.Add(attributeNameOrIndex, attributeValue);
	}

	public void AddMatric(string metricNameOrIndex, double metricValue)
	{
		if(_matrics == null)
			_matrics = new Dictionary<string, double>();
		
		_matrics.Add(metricNameOrIndex, metricValue);
	}
}
