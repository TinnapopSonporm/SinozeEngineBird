using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public class PlatformBuildDefines
{
	public static void Add(string define)
	{
		define = define.ToUpper();
		var list = Get ();
		if(list.Contains(define))
			return;
		list.Add(define);
		Set(list);
	}

	public static void Remove(string define)
	{
		define = define.ToUpper();
		var list = Get ();
		if(!list.Contains(define))
			return;
		list.Remove(define);
		Set(list);
	}

	public static void Toggle(string define)
	{
		define = define.ToUpper();
		var list = Get ();
		if(list.Contains(define))
			list.Remove(define);
		else
			list.Add(define);
		Set(list);
	}

	public static void Enable(string define, bool isEnabled)
	{
		if(isEnabled)
			Add(define);
		else
			Remove(define);
	}

	public static void Clear()
	{
		PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentBuildTargetGroup, "");
	}

	private static List<string> Get()
	{
		var s = PlayerSettings.GetScriptingDefineSymbolsForGroup(CurrentBuildTargetGroup);
		var ss = s.Split(';');
		var result = new List<string>();
		foreach(var sss in ss)
		{
			result.Add(sss.ToUpper());
		}
		return result;
	}

	private static void Set(List<string> defines)
	{
		string result = "";
		for(int i=0;i<defines.Count;i++)
		{
			if(i != 0)
				result += ";";
			result += defines[i].ToUpper();
		}
		PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentBuildTargetGroup, result);
	}

	private static BuildTargetGroup CurrentBuildTargetGroup
	{
		get
		{
			switch(EditorUserBuildSettings.activeBuildTarget)
			{
				#if UNITY_5
				case BuildTarget.iOS: return BuildTargetGroup.iOS;
				#else
				case BuildTarget.iPhone: return BuildTargetGroup.iPhone;
				#endif
				case BuildTarget.Android: return BuildTargetGroup.Android;
				default: throw new NotImplementedException("add more build target group");
			}
		}
	}
}
