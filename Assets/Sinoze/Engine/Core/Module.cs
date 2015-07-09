
public static class Module 
{
	public static T Find<T>()
	{
		return Sinoze.Engine.EngineRoot.Instance.GetModule<T>();
	}
}
