#if UNITY_5
using System.Collections;

public static class UnityAsset
{
	// shortcut
	public static UnityLoadJob Load(LoadInstruction instruction)
	{
		return (UnityLoadJob)Module.Find<SinozeAssetLoader>().Load(instruction);
	}
	
	public static LoadJobCollection Load(LoadInstructionCollection instructions)
	{
		return Module.Find<SinozeAssetLoader>().Load(instructions);
	}
}
#endif