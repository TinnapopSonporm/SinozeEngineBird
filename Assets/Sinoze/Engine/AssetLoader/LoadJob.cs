
using System;
using UnityEngine;

public abstract class LoadJob : IDisposable
{
	public LoadInstruction Instruction { get; private set; }
	public object Asset { get; protected set; }
	public bool IsDisposed { get; private set; }
	public bool IsStarted { get; private set; }
	public bool IsDoneSuccess { get; protected set; }
	public bool IsDoneFailure { get; protected set; }
	public bool IsDone { get { return IsDoneSuccess || IsDoneFailure; }}
	public float Progress { get; protected set; }
	public ThreadPriority priority { get; set;}
	public int FileSize { get; set;}
	public LoadJobCollection ParentCollection { get; internal set; }

	public void Start()
	{
		if(IsStarted)
		{
			return;
		}
		LoadStart();
	}
	protected abstract void LoadStart();
	public abstract void LoadUpdate();

	public LoadJob(LoadInstruction instruction)
	{
		this.Instruction = instruction;
	}

	public void Dispose()
	{
		if(IsDisposed)
		{
			return;
		}

		if(Asset != null && Asset is IDisposable)
		{
			(Asset as IDisposable).Dispose ();
			Asset = null;
		}

		IsDisposed = true;
	}
}

public abstract class LoadJob<T> : LoadJob where T : IDisposable
{
	public new T Asset 
	{ 
		get { return (T)base.Asset; }
		protected set { base.Asset = value; }
	}
	
	public LoadJob(LoadInstruction instruction)
		:base(instruction)
	{
	}
}
