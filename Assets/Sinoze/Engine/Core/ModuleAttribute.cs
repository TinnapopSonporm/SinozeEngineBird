using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sinoze.Engine
{
	/// <summary>
	/// Sinoze engine component attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	class ModuleAttribute : Attribute
	{
		public Type Service { get; private set; }
		public bool EditorModeOnly { get; private set; }

		private static IEnumerable<Type> _modulesCache;
		private static Dictionary<Type, List<Type>> _adaptorsCache;
		
		public static Dictionary<Type, List<Type>> GetAdaptorModules()
		{
			if(_adaptorsCache == null)
				_adaptorsCache = new Dictionary<Type, List<Type>>();
			else
				return _adaptorsCache;

			var adaptors = _GetAdaptorModules();
			
			foreach(var adaptor in adaptors)
			{
				List<Type> adaptableModules;
				if(!_adaptorsCache.TryGetValue(adaptor, out adaptableModules))
				{
					adaptableModules = new List<Type>();
					_adaptorsCache.Add(adaptor, adaptableModules);
				}
				
				var attr = adaptor.GetCustomAttributes(typeof(ModuleAttribute), true);
				foreach(var a in attr)
				{
					var at = a as ModuleAttribute;
					adaptableModules.Add(at.Service);
				}
			}

			return _adaptorsCache;
		}

		// all
		public static IEnumerable<Type> GetAllModules()
		{
			if(_modulesCache == null)
			{
				var list = new List<Type>();
				var temp = AttributeHelper.GetTypesWithAttribute<ModuleAttribute>();
				foreach(var t in temp)
				{
					var attr = t.GetCustomAttributes(typeof(ModuleAttribute), true);
					bool isValid = true;
					foreach(var a in attr)
					{
						// skip module that is EditorModeOnly
						var at = a as ModuleAttribute;
						if(at.EditorModeOnly)
						{
							#if !UNITY_EDITOR
							isValid = false;
							#endif
						}
					}
					if(!isValid)
					{
						continue;
					}
					list.Add(t);
				}
				_modulesCache = list;
			}
			return _modulesCache;
		}

		private static IEnumerable<Type> _GetAdaptorModules()
		{
			var modules = GetAllModules();

			foreach(var module in modules)
			{
				var attr = module.GetCustomAttributes(typeof(ModuleAttribute), true);
				foreach(var a in attr)
				{
					var at = a as ModuleAttribute;
					if(at.Service != null)
					{
						yield return module;
						break;
					}
				}
			}
		}
	}
}