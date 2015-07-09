using Sinoze.Engine;
using System.Collections.Generic;

public sealed partial class SinozeAssetLoader : IUpdatable
{
	private Queue<LoadJob> pendingJobs = new Queue<LoadJob>();
	private List<LoadJob> loadingJobs = new List<LoadJob>();
	private Queue<LoadJob> removingJobs = new Queue<LoadJob>();

	private void EnqueueJob(LoadJob loadResult)
	{
		lock(pendingJobs)
		{
			pendingJobs.Enqueue(loadResult);
		}
	}
	
	#region IUpdatable implementation
	
	public void Update ()
	{
		foreach(var loadingJob in loadingJobs)
		{
			loadingJob.LoadUpdate();

			// done (success, or fully failed/no more retry)
			if(loadingJob.IsDone)
				removingJobs.Enqueue(loadingJob);
		}

		while(removingJobs.Count > 0)
		{
			loadingJobs.Remove(removingJobs.Dequeue ());
		}
	}
	
	public void LateUpdate ()
	{
		while(pendingJobs.Count > 0)
		{
			var loadJob = pendingJobs.Dequeue();
			loadingJobs.Add(loadJob);
			loadJob.Start();
		}
	}
	
	#endregion
}
