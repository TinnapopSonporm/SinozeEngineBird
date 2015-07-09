#if UNITY_5
using UnityEditor;

public class CreateAssetBundles
{
	[MenuItem ("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles ()
	{
		BuildPipeline.BuildAssetBundles ("Build/AssetBundles",  BuildAssetBundleOptions.None,  BuildTarget.StandaloneOSXUniversal);
	}
}
#endif