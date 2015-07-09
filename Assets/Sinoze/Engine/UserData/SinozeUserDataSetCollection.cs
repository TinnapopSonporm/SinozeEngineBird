using System.Collections.Generic;

public class SinozeUserDataSetCollection
{
	public SinozeUserDataSetCollection(SinozeUser owner)
	{
		this.Owner = owner;
	}

	public SinozeUser Owner
	{
		get; private set;
	}

	Dictionary<string, SinozeUserDataSet> datasets = new Dictionary<string, SinozeUserDataSet>();

	public SinozeUserDataSet this[string dataSetName]
	{
		get
		{
			SinozeUserDataSet dataset;
			if(!datasets.TryGetValue(dataSetName, out dataset))
			{
				dataset = new SinozeUserDataSet(this, dataSetName);
				datasets.Add(dataSetName, dataset);
			}
			return dataset;
		}
	}

	public int Count
	{
		get { return datasets.Count; }
	}
}