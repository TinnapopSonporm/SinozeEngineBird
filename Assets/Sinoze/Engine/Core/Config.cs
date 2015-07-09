using System;
using System.Collections;
using System.Collections.Generic;

namespace Sinoze.Engine
{
	[Serializable]
	public class Config
	{	
		// serialized by unity
		public string globalConfigValue = null;
		public string[] customConfigTypes = new string[0];
		public string[] customConfigValues = new string[0];

		// runtime cache
		private Dictionary<Type, object> _cachedCustomConfigs;
		private GlobalConfig _cachedGlobalConfig;
		public GlobalConfig Global 
		{
			get
			{
				if(_cachedGlobalConfig == null)
				{
					if(!string.IsNullOrEmpty(globalConfigValue))
						_cachedGlobalConfig = XmlHelper.Deserialize<GlobalConfig>(globalConfigValue);
					else
						_cachedGlobalConfig = new GlobalConfig();
				}
				return _cachedGlobalConfig;
			}
		}

		// PUBLIC API
		public static T Find<T>()
		{
			var instance = EngineRoot.Instance.Config;
			if(instance == null)
				return default(T);

			if(instance._cachedCustomConfigs == null)
				instance._cachedCustomConfigs = new Dictionary<Type, object>();

			object obj;
			var t = typeof(T);
			if(!instance._cachedCustomConfigs.TryGetValue(t, out obj))
			{
				bool found = false;
				for(int i=0;i<instance.customConfigTypes.Length;i++)
				{
					if(t.AssemblyQualifiedName.Equals(instance.customConfigTypes[i]))
					{
						obj = XmlHelper.Deserialize(t, instance.customConfigValues[i]);
						found = true;
						break;
					}
				}
				if(!found)
					obj = Activator.CreateInstance<T>();
				instance._cachedCustomConfigs.Add(t, obj);
			}
			return (T)obj;
		}
	}


	// for global use
	public class GlobalConfig
	{
		public string startSceneName;
	}
}
