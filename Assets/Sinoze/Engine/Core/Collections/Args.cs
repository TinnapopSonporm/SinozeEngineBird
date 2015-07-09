using System.Collections;

namespace Sinoze.Engine
{
	public struct Args<T1, T2>
	{
		public T1 arg1;
		public T2 arg2;

		public Args(T1 arg1, T2 arg2)
		{
			this.arg1 = arg1;
			this.arg2 = arg2;
		}
		public Args(ref T1 arg1, ref T2 arg2)
		{
			this.arg1 = arg1;
			this.arg2 = arg2;
		}
	}

	public struct Args<T1, T2, T3>
	{
		public T1 arg1;
		public T2 arg2;
		public T3 arg3;
		
		public Args(T1 arg1, T2 arg2, T3 arg3)
		{
			this.arg1 = arg1;
			this.arg2 = arg2;
			this.arg3 = arg3;
		}
		public Args(ref T1 arg1, ref T2 arg2, ref T3 arg3)
		{
			this.arg1 = arg1;
			this.arg2 = arg2;
			this.arg3 = arg3;
		}
	}

	public struct Args<T1, T2, T3, T4>
	{
		public T1 arg1;
		public T2 arg2;
		public T3 arg3;
		public T4 arg4;
		
		public Args(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			this.arg1 = arg1;
			this.arg2 = arg2;
			this.arg3 = arg3;
			this.arg4 = arg4;
		}
		public Args(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4)
		{
			this.arg1 = arg1;
			this.arg2 = arg2;
			this.arg3 = arg3;
			this.arg4 = arg4;
		}
	}
	
	public struct Args<T1, T2, T3, T4, T5>
	{
		public T1 arg1;
		public T2 arg2;
		public T3 arg3;
		public T4 arg4;
		public T5 arg5;
		
		public Args(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			this.arg1 = arg1;
			this.arg2 = arg2;
			this.arg3 = arg3;
			this.arg4 = arg4;
			this.arg5 = arg5;
		}
		public Args(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4, ref T5 arg5)
		{
			this.arg1 = arg1;
			this.arg2 = arg2;
			this.arg3 = arg3;
			this.arg4 = arg4;
			this.arg5 = arg5;
		}
	}
}