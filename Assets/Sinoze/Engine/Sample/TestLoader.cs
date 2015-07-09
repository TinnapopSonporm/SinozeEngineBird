#if UNITY_5
using UnityEngine;
using Sinoze.Engine;
using System.Collections;

[Module]
public class TestLoader : MonoBehaviour 
{
	UnityLoadJob job;
	string url = "";
	GameObject plane;
	void Start()
	{
		//QualitySettings.SetQualityLevel(5);
		plane = GameObject.Find("AAA");
	}
	void OnGUI()
	{
		//if(job == null)
		{
			url = GUILayout.TextField(url);
			if(GUILayout.Button("Load"))
			{
				url = LoaderHelper.CheckPath(url);
				var inst = new LoadInstruction();
				inst.url = url;
				inst.priority = ThreadPriority.High;
				//inst.policy = new LoadJobPolicy(5, 1000);
				job = UnityAsset.Load(inst);
			}
		}
		//else
		if(job != null)
		{
			//GUILayout.Label("Size = " + job.FileSize + "/" + job.Progress.ToString ("0.0%"));
			Debug.Log ("Size = " + ((job.FileSize) * job.Progress)  + "/" + job.FileSize + " : Progress [" + job.Progress.ToString ("0.0%") + "]");
			if(job.IsDoneSuccess)
			{
				if( job.Asset.assetBundle != null)
				{
				var p  = job.Asset.assetBundle.GetAllScenePaths();
				if(p.Length == 1)
				{
					Application.LoadLevelAdditiveAsync(System.IO.Path.GetFileNameWithoutExtension(p[0]));
				}
				}
				//Logger.Log(job.Asset.text);

				Debug.Log ("Download Piority = " + job.priority);
				Renderer renderer = plane.GetComponent<Renderer>();
				renderer.material.mainTexture = job.Asset.texture;


				//Saver.SaveTextureToFile(job.Asset.texture, "Test", Saver.IMAGE_TYPE.PNG);
				job = null;
			}
		}
	}

	void Update() {



	}
}
#endif