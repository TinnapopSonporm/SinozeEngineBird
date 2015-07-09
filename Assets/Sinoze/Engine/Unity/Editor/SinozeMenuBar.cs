using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using Sinoze.Engine.Unity;

namespace Sinoze.Engine.Editor
{
	/// <summary>
	/// Sinoze menu bar.
	/// </summary>
	class SinozeMenuBar
	{	
		[MenuItem ("Sinoze/Game Setup", false, 0)]
		static void StartGame()
		{
			// show game setup dialog
			EditorWindow.GetWindow<GameSetupDialog>(true, "Game Setup");
		}
	}
}