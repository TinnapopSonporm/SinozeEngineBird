using Sinoze.Engine;
using System;
using System.Collections.Generic;

public delegate void SinozeUserChangedDelegate(SinozeUser oldUser, SinozeUser newUser);

[Module]
public class SinozeUserIdentity : AdaptableService, IUpdatable
{
	private const int MAX_USERS_COUNT = 4;
	
	public SinozeUserIdentity()
	{
		if(_instance != null)
			throw new InvalidOperationException("SinozeUserPool already instanciated");
		_instance = this;
		
		// auto get or create first user
		//SinozeUserPool.GetUserAsync(0, (u) => {UnityEngine.Debug.Log("default = " + u.UserID);});
	}

	#region IUpdatable implementation

	public void Update ()
	{
		//TODO: optimize this
		FetchPendingJobs();
		foreach(var queue in _jobQueues)
		{
			if(queue.Value.HasJobReadyToExecute)
			{
				queue.Value.ExecuteNextJob();
			}
		}
	}

	public void LateUpdate ()
	{
		foreach(var queue in _jobQueues)
		{
			queue.Value.CallPendingResults();
		}
	}

	#endregion

	static SinozeUserIdentity _instance;

	Dictionary<int, SinozeUser> _users = new Dictionary<int, SinozeUser>();
	Dictionary<int, GetUserAsyncJobQueue> _jobQueues = new Dictionary<int, GetUserAsyncJobQueue>();

	#region PUBLIC API
	public static event SinozeUserChangedDelegate UserCreatedOrChanged;

	public static bool IsUserExists(int userIndex)
	{
		return _instance._users.ContainsKey(userIndex);
	}

	public static void GetOrCreateUserAsync(int userIndex, Action<SinozeUser> userResult)
	{
		_instance.EnqueueGetUserAsyncJob(userIndex, userResult);
	}

	#endregion

	Queue<GetUserAsyncJobQueue.GetUserAsyncJob> pendingJobs = new Queue<GetUserAsyncJobQueue.GetUserAsyncJob>();
	private void EnqueueGetUserAsyncJob(int userIndex, Action<SinozeUser> userResult, Dictionary<ThirdPartyLogin, string> thirdPartyLogins = null)
	{
		var newJob = new GetUserAsyncJobQueue.GetUserAsyncJob()
		{
			userIndex = userIndex,
			userResult = userResult,
			thirdPartyLogins = thirdPartyLogins,
		};
		pendingJobs.Enqueue(newJob);
	}

	private void FetchPendingJobs()
	{
		while(pendingJobs.Count > 0)
		{
			var job = pendingJobs.Dequeue ();
			GetUserAsyncJobQueue queue;
			if(!_jobQueues.TryGetValue(job.userIndex, out queue))
			{
				queue = new GetUserAsyncJobQueue(this);
				_jobQueues.Add(job.userIndex, queue);
			}
			queue.EnqueueJob(job);
		}
	}

	class GetUserAsyncJobQueue
	{
		private SinozeUserIdentity parent;
		private Queue<GetUserAsyncJob> _jobQueue = new Queue<GetUserAsyncJob>();
		private GetUserAsyncJob executingJob;

		public bool HasJobReadyToExecute { get { return _jobQueue.Count > 0 && executingJob == null; } }

		public GetUserAsyncJobQueue(SinozeUserIdentity parent)
		{
			this.parent = parent;
		}

		public void EnqueueJob(GetUserAsyncJob job)
		{
			job.parent = this;
			_jobQueue.Enqueue(job);
		}

		public void ExecuteNextJob()
		{
			executingJob = _jobQueue.Dequeue();
			executingJob.Execute();
		}

		public void CallPendingResults()
		{
			if(executingJob != null && executingJob.IsResultRetrived)
			{
				executingJob.InvokeResult();
				executingJob = null;
			}
		}

		public class GetUserAsyncJob
		{
			public GetUserAsyncJobQueue parent;

			public int userIndex;
			public Action<SinozeUser> userResult;
			public Dictionary<ThirdPartyLogin, string> thirdPartyLogins;
			public bool IsResultRetrived { get { return _isResultRetrived; }}
			
			private bool _isExecutionStarted;
			private bool _isResultRetrived;
			private bool _isResultInvoked;
			private SinozeUser result;

			private void UserRetrived(SinozeUser user)
			{
				this.result = user;
				_isResultRetrived = true;
			}

			public void Execute()
			{
				if(_isExecutionStarted || _isResultRetrived)
					throw new InvalidOperationException("GetUserAsyncJob already executed");
				_isExecutionStarted = true;

				// get new userid (or look for existing user)
				parent.parent.GetIdAsync(userIndex, thirdPartyLogins, (userId)=>
				{
					if(parent.parent._users.ContainsKey(userIndex) && parent.parent._users[userIndex].Identity == userId)
					{
						// id unchanged
						this.UserRetrived(parent.parent._users[userIndex]);
						return;
					}

					// id changed, create new user instance
					SinozeUser newuser = new SinozeUser(userIndex, userId);
					this.UserRetrived(newuser);
				});
			}

			public void InvokeResult()
			{
				if(_isResultInvoked)
					throw new InvalidOperationException("GetUserAsyncJob result already invoke");
				_isResultInvoked = true;

				parent.parent.StoreUser(result);
				userResult(result);
			}
		}
	}

	private void StoreUser(SinozeUser user)
	{
		var index = user.Index;
		if(_users.ContainsKey(index))
		{
			var oldUser = _users[index];
			if(oldUser != user)
			{
				_users[index] = user;
				OnUserCreatedOrChanged(oldUser, user);
			}
		}
		else
		{
			_users.Add(index, user);
			OnUserCreatedOrChanged(null, user);
		}
	}

	private void OnUserCreatedOrChanged(SinozeUser oldUser, SinozeUser newUser)
	{
		if(UserCreatedOrChanged != null)
		{
			UserCreatedOrChanged(oldUser, newUser);
		}
	}

	private void GetIdAsync(int userIndex, Dictionary<ThirdPartyLogin, string> thirdPartyLogins, Action<string> idResult)
	{
		if(adaptors.Count == 1)
		{
			InvokeAdaptors("GetIdAsync", userIndex, thirdPartyLogins, idResult);
		}
		else
		{
			idResult("DEFAULTUSERID");
		}
	}

	internal void Resync(SinozeUser currentUser, Action<SinozeUser> result)
	{
		EnqueueGetUserAsyncJob(currentUser.Index, (user)=>{ result(user); }, currentUser.ThirdPartyLogins);
	}
}