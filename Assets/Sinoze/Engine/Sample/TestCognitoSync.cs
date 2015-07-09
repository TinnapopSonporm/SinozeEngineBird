using UnityEngine;
using System.Collections;
using Sinoze.Engine;

//[Module]
public class TestCognitoSync : MonoBehaviour 
{
	void Sync(int userindex)
	{
		SinozeUserIdentity.GetOrCreateUserAsync(userindex, (user) =>
		{
			Debug.Log("user" + user.Index + " : " + user.Identity);
			
			//user.AddThirdPartyLogin(ThirdPartyLogin.Facebook, "CAAUlikaAMLQBAOP5vixvRaJIDCzzEsZBUjOVPrQPBB667rw3F5OMjPZCayj6NL7SsKGu0bWbfUZAs8toNHdZBCMLvr2DJpWg2E5G5E9p0MOZC459qCF80KQummKCosuNxBXiuVqpf7sDAxTNZBkMac81CtMQPHXTc61U8XAjT8A0TO0RkQVCFWlcd1GRJEEbXZB4JU5UHvTwxPIu463fpB6",
			//(r) =>
			//{

			user.DataSets["dataset1"].Sync((r) =>
			{
				Debug.Log("id = " + user.Identity);
				Debug.Log("synce result " + userindex + " = " + r);
			});
		});
	}

	void Get(int userindex)
	{
		SinozeUserIdentity.GetOrCreateUserAsync(userindex, (user) =>
		{
			var data = user.DataSets["dataset1"].Get("data1");
			Debug.Log("id = " + user.Identity);
			Debug.Log("get data" + userindex + " = " + data);
		});
	}
	
	void Put(int userindex, string value)
	{
		SinozeUserIdentity.GetOrCreateUserAsync(userindex, (user) =>
		{
			user.DataSets["dataset1"].Put("data1", value);
		});
	}

	void OnGUI()
	{
		if(GUILayout.Button("Sync 0"))
			Sync(0);
		if(GUILayout.Button("Sync 1"))
			Sync(1);
		if(GUILayout.Button("Sync 2"))
			Sync(2);
		if(GUILayout.Button("Sync 3"))
			Sync(3);
		
		
		if(GUILayout.Button("Get 0"))
			Get(0);
		if(GUILayout.Button("Get 1"))
			Get(1);
		if(GUILayout.Button("Get 2"))
			Get(2);
		if(GUILayout.Button("Get 3"))
			Get(3);
		
		if(GUILayout.Button("Put 0"))
			Put(0, "value0");
		if(GUILayout.Button("Put 1"))
			Put(1, "value1");
		if(GUILayout.Button("Put 2"))
			Put(2, "value2");
		if(GUILayout.Button("Put 3"))
			Put(3, "value3");
	}
}
