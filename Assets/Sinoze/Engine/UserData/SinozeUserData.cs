using UnityEngine;
using System.Collections;
using Sinoze.Engine;
using System;

[Module]
public class SinozeUserData : AdaptableService<IUserDataSync>
{
	public string Get(SinozeUser user, string dataSetName, string key)
	{
		return adaptors[0].Get(user, dataSetName, key);
	}

	public void Put(SinozeUser user, string dataSetName, string key, string value)
	{
		adaptors[0].Put(user, dataSetName, key, value);
	}

	public void Sync(SinozeUser user, string dataSetName, Action<bool> result)
	{
		adaptors[0].Sync(user, dataSetName, result);
	}
}

public interface IUserDataSync
{
	string Get(SinozeUser user, string dataSetName, string key);
	
	void Put(SinozeUser user, string dataSetName, string key, string value);
	
	void Sync(SinozeUser user, string dataSetName, Action<bool> result);
}