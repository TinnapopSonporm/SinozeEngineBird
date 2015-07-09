using System;

public interface IPlatformHook
{
	object CreateInstance(Type type, object context);
	void DestroyInstance(object instance, object context);
}
