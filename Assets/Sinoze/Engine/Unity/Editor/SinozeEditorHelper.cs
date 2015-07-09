using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Sinoze.Engine.Editor
{
	public static class SinozeEditorHelper
	{
		public const string BootstrapScenePath = "Assets/Bootstrap (Auto-Generated).unity";

		/// <summary>
		/// Validates the build setting scenes my making sure that Bootstrap is listed there at the first item
		/// </summary>
		public static void ValidateBuildSettingScenes(string startScenePath)
		{
			var bootstrapScenePath = BootstrapScenePath.ToLower();
			bool bootstrapSceneFound = false;
			bool bootstrapSceneDisabled = false;
			int bootstrapSceneIndex = -1;
			var startScenePathLower = startScenePath.ToLower();
			bool startSceneFound = false;
			bool startSceneDisabled = false;
			int startSceneIndex = -1;
			
			for(int i=0;i<UnityEditor.EditorBuildSettings.scenes.Length;i++)
			{
				var s = UnityEditor.EditorBuildSettings.scenes[i];
				var sLower = s.path.ToLower();
				if(sLower == bootstrapScenePath && !bootstrapSceneFound)
				{
					bootstrapSceneFound = true;
					bootstrapSceneDisabled = !s.enabled;
					bootstrapSceneIndex = i;
				}
				else if(sLower == startScenePathLower && !startSceneFound)
				{
					startSceneFound = true;
					startSceneDisabled = !s.enabled;
					startSceneIndex = i;
				}
			}

			if(startSceneDisabled)
			{
				// make sure the start scene is enabled
				EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length];
				System.Array.Copy(original, newSettings, original.Length);
				
				// enable
				newSettings[startSceneIndex].enabled = true;
				UnityEditor.EditorBuildSettings.scenes = newSettings;
			}
			
			if(bootstrapSceneDisabled || bootstrapSceneIndex != -1)
			{
				// make sure the bootstrap scene is in the first and enabled
				EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length];
				System.Array.Copy(original, newSettings, original.Length);
				
				// enable
				newSettings[bootstrapSceneIndex].enabled = true;
				
				// swap to first
				var tmp = newSettings[0];
				newSettings[0] = newSettings[bootstrapSceneIndex];
				newSettings[bootstrapSceneIndex] = tmp;
				
				// update
				UnityEditor.EditorBuildSettings.scenes = newSettings;
			}
			
			if(!bootstrapSceneFound)
			{
				// bootstrap scene not found in settings
				EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length + 1];
				System.Array.Copy(original, 0, newSettings, 1, original.Length);
				
				// put bootstrap scene at the first item
				newSettings[0] = new EditorBuildSettingsScene(bootstrapScenePath, true);
				UnityEditor.EditorBuildSettings.scenes = newSettings;
			}
			
			if(!startSceneFound)
			{
				// start scene not found in settings
				EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length + 1];
				System.Array.Copy(original, newSettings, original.Length);
				
				// put start scene scene at the last item
				newSettings[newSettings.Length - 1] = new EditorBuildSettingsScene(startScenePath, true);
				UnityEditor.EditorBuildSettings.scenes = newSettings;
			}
			
		}
	}
}