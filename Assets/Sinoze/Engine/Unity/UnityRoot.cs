using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Sinoze.Engine.Unity
{
	public class UnityRoot : UnityPlatformHook
	{
		public static UnityRoot Instance { get; private set; }
		private EngineRoot _engine;

		#region Bootstrap
		internal static void Init(Config config)
		{
			if(Instance != null)
				throw new System.InvalidOperationException("UnityRoot already instantiated");

			var rootGameObject = new GameObject();
			GameObject.DontDestroyOnLoad(rootGameObject);

			Instance = rootGameObject.AddComponent<UnityRoot>();
			Instance.StartEngine(config);
		}
		#endregion

		void StartEngine(Config config)
		{
			_engine = new EngineRoot(this, config);
			_engine.Start();
		}

		void Update()
		{
			_engine.Environment.currentFrameIndex = Time.frameCount;
			_engine.Update();
		}

		void LateUpdate()
		{
			_engine.LateUpdate();
		}
	}
}