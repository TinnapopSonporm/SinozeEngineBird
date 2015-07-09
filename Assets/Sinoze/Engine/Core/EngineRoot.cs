using System.Collections;
using System.Collections.Generic;
using Sinoze.Engine;
using System;
using System.Collections.ObjectModel;

namespace Sinoze.Engine
{
	public class EngineRoot
	{
		public static EngineRoot Instance { get; private set; }

		public Config Config { get; private set; }
		public EngineEnvironment Environment { get; private set; }
		
		private IPlatformHook hook;
		private List<IUpdatable> updatables = new List<IUpdatable>();
		private Dictionary<Type, object> _modules = new Dictionary<Type, object>();
		private Dictionary<object, List<AdaptableService>> _adaptor_service_map = new Dictionary<object, List<AdaptableService>>();
		private Queue<object> moduleToRemove = new Queue<object>();

		public EngineRoot(IPlatformHook hook, Config config)
		{
			if(Instance != null)
				throw new System.InvalidOperationException("EngineRoot already instantiated");

			this.hook = hook;
			this.Config = config;
			this.Environment = new EngineEnvironment();
			Instance = this;
		}

		// hook to platform
		public void Start()
		{
			// create all module instances
			foreach(var type in ModuleAttribute.GetAllModules())
			{
				var module = hook.CreateInstance(type, this);
				_modules.Add(type, module);
				if(module is IUpdatable)
					updatables.Add(module as IUpdatable);
			}

			// link adaptors to their services
			foreach(var adaptor in ModuleAttribute.GetAdaptorModules())
			{
				foreach(var serviceType in adaptor.Value)
				{
					var service = _modules[serviceType] as AdaptableService;
					if(service == null)
						throw new InvalidCastException(adaptor.Key + " must derive from " + typeof(AdaptableService));

					service.AttachAdaptor(_modules[adaptor.Key]);

					List<AdaptableService> services;
					if(!_adaptor_service_map.TryGetValue(_modules[adaptor.Key], out services))
					{
						services = new List<AdaptableService>();
						_adaptor_service_map.Add(_modules[adaptor.Key], services);
					}
					services.Add(service);
				}
			}
		}

		public void RemoveModule(object module)
		{
			// delay to call PRIVATE_RemoveModule in LateUpdate()
			moduleToRemove.Enqueue(module);
		}

		private void PRIVATE_RemoveModule(object module)
		{
			_modules.Remove(module.GetType());
			if(module is IUpdatable)
				updatables.Remove(module as IUpdatable);
			List<AdaptableService> services;
			if(_adaptor_service_map.TryGetValue(module, out services))
			{
				foreach(var service in services)
					service.DetachAdaptor(module);
			}
		}
		
		// hook to platform
		public void Update()
		{
			foreach(var u in updatables)
				u.Update();
		}
		
		// hook to platform
		public void LateUpdate()
		{
			foreach(var u in updatables)
				u.LateUpdate();

			while(moduleToRemove.Count > 0)
				PRIVATE_RemoveModule(moduleToRemove.Dequeue());
		}

		public object GetModule(Type type)
		{
			return _modules[type];
		}

		public T GetModule<T>()
		{
			return (T)GetModule(typeof(T));
		}
	}
}