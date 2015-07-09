using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sinoze.Engine
{
	public abstract class AdaptableService<T> : AdaptableService
	{
		new protected List<T> adaptors = new List<T>();

		protected override void AddNewAdaptor (object adaptor)
		{
			adaptors.Add((T)adaptor);
		}

		protected override bool IsValidAdaptor (object adaptor)
		{
			var type = adaptor.GetType();
			return typeof(T).IsAssignableFrom(type) && typeof(T) != type;
		}
	}

	public abstract class AdaptableService
	{
		protected List<object> adaptors = new List<object>();

		internal void AttachAdaptor(object adaptor)
		{
			if(IsValidAdaptor(adaptor))
			{
				AddNewAdaptor(adaptor);
			}
			else
			{
				throw new InvalidOperationException("invalid adaptor type " + adaptor.GetType() + " for service " + this.GetType());
			}
		}

		internal void DetachAdaptor(object adaptor)
		{
			adaptors.Remove(adaptor);
		}

		protected virtual void AddNewAdaptor(object adaptor)
		{
			adaptors.Add(adaptor);
		}

		protected virtual bool IsValidAdaptor(object adaptor)
		{
			return true;
		}
		
		protected object InvokeAdaptor(string methodName, params object[] parameters)
		{
			return adaptors[0].GetType ().GetMethod(methodName).Invoke(adaptors[0], parameters);
		}

		protected void InvokeAdaptors(string methodName, params object[] parameters)
		{
			foreach(var adaptor in adaptors)
			{
				adaptor.GetType ().GetMethod(methodName).Invoke(adaptor, parameters);
			}
		}
	}
}