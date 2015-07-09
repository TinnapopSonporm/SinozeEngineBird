using Sinoze.Engine;
using Sinoze.Engine.Unity;

#if AWS
[Module]
public class AWSUnity  
{
	public AWSUnity()
	{ 
		UnityRoot.Instance.gameObject.AddComponent<Amazon.UnityInitializer>();
	}
}
#endif

[Config(Name = "AWS", Define = "AWS")]
public class AwsConfig
{
	public string cognitoID;
}