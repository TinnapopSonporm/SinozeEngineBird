// v2.0.0.1

using Sinoze.Engine;

#if AWS_COGNITOSYNC
using UnityEngine;
using System.Collections;
using Amazon.CognitoSync.SyncManager.Internal;
using Amazon.Runtime;
using Amazon.CognitoSync;
using Amazon.CognitoSync.Model;
using Amazon;
using Amazon.CognitoIdentity;
using System.Collections.Generic;
using Amazon.CognitoSync.SyncManager;
using System;

[Module(Service = typeof(SinozeUserData))]
public class AWSCognitoSyncAdaptor : MonoBehaviour, IUserDataSync
{
	AWSCognitoAdaptor cognito;
	Dictionary<string, CognitoSyncManager> syncManagers = new Dictionary<string, CognitoSyncManager>();

	void Start()
	{ 
		cognito = Module.Find<AWSCognitoAdaptor>();
	}

	public string Get(SinozeUser user, string dataSetName, string key)
	{
		var dataSet = GetDataset(user.Identity, dataSetName);
		var result = dataSet.Get(key);
		dataSet.Dispose();
		return result;
	}
	
	public void Put(SinozeUser user, string dataSetName, string key, string value)
	{
		var dataSet = GetDataset(user.Identity, dataSetName);
		dataSet.Put(key, value);
		dataSet.Dispose ();
	}
	
	public void Sync(SinozeUser user, string dataSetName, Action<bool> result)
	{
		var dataSet = GetDataset(user.Identity, dataSetName);

		dataSet.OnSyncSuccess += (object sender, SyncSuccessEvent e) => 
		{
			if(result != null)
				result(true);
			dataSet.Dispose ();
		};
		dataSet.OnSyncFailure += (object sender, SyncFailureEvent e) => 
		{
			if(result != null)
				result(false);
			dataSet.Dispose ();
		};

		dataSet.Synchronize();
	}

	private Amazon.CognitoSync.SyncManager.Dataset GetDataset(string userId, string dataSetName)
	{
		return GetSyncManager(userId).OpenOrCreateDataset(dataSetName);
	}

	public CognitoSyncManager GetSyncManager(string userId)
	{
		var credential = cognito.GetCredential(userId);

		CognitoSyncManager syncManager;
		if(!syncManagers.TryGetValue(userId, out syncManager))
		{
			syncManager = new CognitoSyncManager(credential, RegionEndpoint.USEast1);
			syncManagers.Add(userId, syncManager);
		}

		return syncManager;
	}
}
#endif

[Config(Name = "AWS Cognito Sync", 
        Define = "AWS_COGNITOSYNC", 
        Dependency = typeof(AwsConfig))]
public class AwsCognitoSyncConfig
{
}