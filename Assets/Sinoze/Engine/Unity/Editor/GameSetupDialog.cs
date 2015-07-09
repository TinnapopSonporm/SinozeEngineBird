using UnityEngine;
using UnityEditor;
using Sinoze.Engine.Unity;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Sinoze.Engine.Editor
{
	partial class GameSetupDialog : EditorWindow 
	{
		GameStartContextEditing engineStartParams;
		Dictionary<Type, GameStartContextEditing> customGameStartParams;

		void OnEnable()
		{
			engineStartParams = new GameStartContextEditing(typeof(GlobalConfig));
			customGameStartParams = new Dictionary<Type, GameStartContextEditing>();

			// search for custom GameStartParameters
			foreach(var type in AttributeHelper.GetTypesWithAttribute<ConfigAttribute>())
			{
				if(!customGameStartParams.ContainsKey(type))
				{
					customGameStartParams.Add(type, new GameStartContextEditing(type));
				}
			}

			// find config dependency
			foreach(var c in customGameStartParams)
			{
				if(c.Value.attribute.Dependency != null)
				{
					c.Value.dependency = customGameStartParams[c.Value.attribute.Dependency];
				}
			}
			
			Load ();
		}

		void OnDisable()
		{
			engineStartParams = null;
			customGameStartParams.Clear ();
			customGameStartParams = null;
		}

		void ConfirmStartGame(string startScenePath)
		{
			// this will check if the current scene is dirty and popup save dialog
			// pass this condition if user click either don't save or save
			// if user click cancel, this will abort
			if(!EditorApplication.SaveCurrentSceneIfUserWantsTo())
				return;

			Save();

			// prepare GameStartContext to have unity auto serialize to the bootstrap scene
			var meta = new Config();
			meta.globalConfigValue = XmlHelper.Serialize(engineStartParams.type, engineStartParams.GetResult());
			var parametersTypes = new List<string>();
			var parametersValues = new List<string>();
			foreach(var d in customGameStartParams)
			{
				if(!d.Value.enabled)
					continue;
				parametersTypes.Add(d.Key.AssemblyQualifiedName);
				parametersValues.Add(XmlHelper.Serialize(d.Key, d.Value.GetResult()));
			}
			meta.customConfigTypes = parametersTypes.ToArray();
			meta.customConfigValues = parametersValues.ToArray();

			// create new scene with bootstrap, apply game setup meta and save the scene
			#if UNITY_5
			EditorApplication.NewEmptyScene();
			#else
			EditorApplication.NewScene();
			#endif
			var bootstrap = new GameObject("Bootstrap").AddComponent<Bootstrap>();
			bootstrap.config = meta;
			EditorApplication.SaveScene(SinozeEditorHelper.BootstrapScenePath);

			// set custom build
			foreach(var d in customGameStartParams)
			{
				var define = d.Value.attribute.Define;
				if(!string.IsNullOrEmpty(define))
					PlatformBuildDefines.Enable(define, d.Value.enabled);
			}

			// make sure the bootstrap scene and the start scene are listed and enabled in build setting
			SinozeEditorHelper.ValidateBuildSettingScenes(startScenePath);

			// start editor playing
			EditorApplication.isPlaying = true;

			Close ();
		}
		
		class GameStartContextEditing
		{
			public Type type;
			private bool _enabled;
			public bool enabled
			{
				get 
				{
					if(dependency != null)
						return dependency.enabled && this._enabled;
					return this._enabled;
				}
				set
				{
					this._enabled = value;
				}
			}
			public bool dependencyEnabled
			{
				get
				{
					if(dependency != null)
						return dependency.enabled;
					return true;
				}
			}
			public ConfigAttribute attribute;
			public List<CustomField> fields = new List<CustomField>();
			public GameStartContextEditing dependency;

			public GameStartContextEditing(Type type)
			{
				this.type = type;
				
				foreach(var f in type.GetFields())
				{
					if(f.IsPublic)
					{
						fields.Add(new CustomField(f));
					}
				}
				
				var attr = type.GetCustomAttributes(typeof(ConfigAttribute), true);
				foreach(var a in attr)
				{
					var at = a as ConfigAttribute;
					this.attribute = at;
				}
			}
			
			public void Load(object loadedObj)
			{
				foreach(var f in fields)
					f.Load(loadedObj);
			}
			
			public object GetResult()
			{
				var result = Activator.CreateInstance(type);

				foreach(var f in fields)
					f.ApplyTo(result);
				
				return result;
			}

			public class CustomField
			{
				enum FieldType
				{
					String, Bool, Int, NotSupported
				}

				FieldType type;
				FieldInfo fieldInfo;
				object value;
				public CustomField(FieldInfo fieldInfo)
				{
					this.fieldInfo = fieldInfo;
					this.value = null;
					this.type = FieldType.NotSupported;

					if(fieldInfo.FieldType == typeof(String))
					{
						type = FieldType.String;
						value = default(String);
					}
					else if(fieldInfo.FieldType == typeof(Boolean))
					{
						type = FieldType.Bool;
						value = default(Boolean);
					}
					else if(fieldInfo.FieldType == typeof(Int32))
					{
						type = FieldType.Int;
						value = default(Int32);
					}
				}
				
				public void Load(object loadedObj)
				{
					value = fieldInfo.GetValue(loadedObj);
				}
				
				public string GetString()
				{
					return value as String;
				}
				
				public void ApplyTo(object obj)
				{
					fieldInfo.SetValue(obj, value);
				}
				
				public void DrawEditor()
				{
					if(type == FieldType.String)
						value = EditorGUILayout.TextField(fieldInfo.Name, GetString());
					else if(type == FieldType.Bool)
						value = EditorGUILayout.Toggle(fieldInfo.Name, (bool)value);
					else if(type == FieldType.Int)
						value = EditorGUILayout.IntField(fieldInfo.Name, (int)value);
					else 
						EditorGUILayout.LabelField(fieldInfo.Name);
				}
			}
		}

	}
}