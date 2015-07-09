using UnityEngine;
using System.Collections;
using System;

namespace Sinoze.Engine.Unity
{
	public class UnityPlatformHook : MonoBehaviour, IPlatformHook
	{
		#region IPlatformHook implementation
		
		public object CreateInstance (Type type, object context)
		{
			if(type.IsSubclassOf(typeof(MonoBehaviour)))
			{
				if(context != null && context is GameObject)
				{
					return (context as GameObject).AddComponent(type);
				}
				else
				{
					return gameObject.AddComponent(type);
				}
			}
			else
			{
				return Activator.CreateInstance(type);
			}
		}
		
		public void DestroyInstance(object instance, object context)
		{
			if(instance is MonoBehaviour)
			{
				Destroy (instance as MonoBehaviour);
			}
		}

		#endregion
	}
}