using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Sinoze.Engine.Unity
{
	/// <summary>
	/// OnAwake summary
	/// 1. Create Root GameObject
	/// 2. Initialize Engine Components (search for SinozeEngineComponentAttribute)
	/// 3. Load Start Scene (if provided)
	/// 
	/// Starting point of the Engine
	/// A game should start with this scene (see "Assets/Sinoze/Engine/Unity/Bootstrap.unity") to have engine components properly setup
	/// However, for development, Bootstrap can also automatically start itself at runtime from any scene, see AutoInit().
	/// </summary>
	public class Bootstrap : MonoBehaviour 
	{
		[SerializeField] // serialized by unity
		public Config config = new Config();

		private bool isAutoCreated;

		static bool _isStarted;

		void Start()
		{
			UnityEngine.Debug.Log("Bootstrap awake!");
			if(_isStarted)
			{
				UnityEngine.Debug.LogWarning("Bootstrap already started -> Abort");
				Exit ();
				return;
			}
			
			// create root engine game object (to hold engine's components)
			if(!_isStarted)
				Init();
			_isStarted = true;
			
			Exit();
		}

		/// <summary>
		/// create root game object and attach engine components
		/// </summary>
		void Init()
		{
			// create root game object
			string name = "Sinoze.Engine";
			if(isAutoCreated)
				name += " (Auto)";

			UnityRoot.Init(config);
			UnityRoot.Instance.gameObject.name = name;

			UnityEngine.Debug.Log("Bootstrap init success!");
			UnityEngine.Debug.Log("------------------------------");
		}

		void Exit()
		{
			if(config == null)
			{
				Destroy(this.gameObject);
				return;
			}

			// boostrap done!
			if(string.IsNullOrEmpty(config.Global.startSceneName))
			{
				Destroy(this.gameObject);
			}
			else
			{
				UnityEngine.Debug.Log("LOAD LEVEL => " + config.Global.startSceneName);
				Application.LoadLevelAsync(config.Global.startSceneName);
			}
		}

		/// <summary>
		/// Only works with Unity5+
		/// When first level is loaded at runtime (after all scripts' Awake() are called), this function will be triggered
		/// TODO: remove this from production since the bootstrap scene should already be properly setup
		/// </summary>
		#if UNITY_5
		[RuntimeInitializeOnLoadMethod]
		#endif
		static void AutoInit()
		{
			// check if needed
			if(UnityRoot.Instance != null)
			{
				// already have root (e.g. created in previous scene and marked with DontDestroyOnLoad), so no need to bootstrap
				UnityEngine.Debug.Log("Bootstrap AutoInit -> Abort");
				return;
			}
			
			// try auto create
			UnityEngine.Debug.Log("Bootstrap AutoInit");
			var bootstrap = GameObject.FindObjectOfType<Bootstrap>();
			if(bootstrap == null)
			{
				UnityEngine.Debug.Log("Sinoze.Engine Bootstrap self-created");
				var obj = new GameObject("Bootstrap-Auto");
				var boot = obj.AddComponent<Bootstrap>();
				boot.isAutoCreated = true;
			}
		}
	}
}