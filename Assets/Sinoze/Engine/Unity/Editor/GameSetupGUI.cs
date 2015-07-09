using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Sinoze.Engine.Editor
{
	public partial class GameSetupDialog
	{
		private string tempErrorMessage;
		
		void OnGUI() 
		{
			if(!string.IsNullOrEmpty(tempErrorMessage))
				EditorGUILayout.LabelField(tempErrorMessage);

			EditorGUILayout.LabelField("Global");
			DrawContext(engineStartParams, true);
			EditorGUILayout.Separator();
			DrawCustomContexts ();
			
			if (GUILayout.Button("Setup & Play")) 
			{
				// try search for the given scene file (note that this is temp value)
				var str = ((GlobalConfig)engineStartParams.GetResult()).startSceneName;
				if(string.IsNullOrEmpty(str)) str = "";
				var tempStartSceneName = str.ToLower();
				var sceneFileName = tempStartSceneName + ".unity";
				var sceneFilePath = "";
				var dotUnityFiles = System.IO.Directory.GetFiles(System.Environment.CurrentDirectory, "*.unity", System.IO.SearchOption.AllDirectories);
				var sceneExist = false;
				foreach(var f in dotUnityFiles)
				{
					if(sceneFileName == System.IO.Path.GetFileName(f).ToLower())
					{
						var filepath = System.IO.Path.GetFullPath(f).ToLower();
						var dir = System.Environment.CurrentDirectory.ToLower();
						if(filepath.StartsWith(dir))
							sceneFilePath = filepath.Remove(0, dir.Length+1);
						sceneExist = true;
						break;
					}
				}
				
				if(sceneExist)
				{
					ConfirmStartGame(sceneFilePath);
				}
				else
				{
					tempErrorMessage = "scene name \"" + tempStartSceneName + "\" does not exist";
				}
			}
			
			if (GUILayout.Button("Save & Close")) 
			{
				Save ();
				Close ();
			}
		}

		void DrawCustomContexts() 
		{
			foreach(var c in customGameStartParams)
			{
				DrawContext(c.Value, false);
				EditorGUILayout.Separator();
			}
		}

		void DrawContext(GameStartContextEditing c, bool forceShow)
		{
			if(!c.dependencyEnabled)
				return;
			var en = c.enabled;
			if(!forceShow)
			{
				en = EditorGUILayout.Toggle (c.attribute.Name ?? c.type.Name, en);
				c.enabled = en;
			}
			if(forceShow || (!forceShow && en))
			{
				foreach(var f in c.fields)
				{
					f.DrawEditor();
				}
			}
		}
	}
}