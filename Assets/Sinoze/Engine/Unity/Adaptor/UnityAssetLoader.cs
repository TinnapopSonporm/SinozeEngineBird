#if UNITY_5
using UnityEngine;
using Sinoze.Engine;

// WWW factory
[Module(Service = typeof(SinozeAssetLoader))]
public class UnityAssetLoader
{
	public UnityLoadJob Load(LoadInstruction instruction)
	{
		return new UnityLoadJob(instruction);
	}
}
#endif