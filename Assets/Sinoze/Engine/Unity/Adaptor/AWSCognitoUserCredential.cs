#if AWS

using UnityEngine;
using System.Collections;
using Amazon.CognitoIdentity;
using System;
using Sinoze.Engine;
using Amazon;
using Amazon.Util.Storage;
using Amazon.Util.Storage.Internal;

public class AWSCognitoUserCredential : CognitoAWSCredentials // extend to support multiple users
{
	int? _userIndex;
	public int UserIndex
	{
		get 
		{
			if(!_userIndex.HasValue)
			{
				if(constructUserIndex == -1)
					throw new InvalidOperationException();
				_userIndex = constructUserIndex;
				constructUserIndex = -1;
			}
			return _userIndex.Value;
		}
	}
	
	private static int constructUserIndex = -1; // hack
	
	public static AWSCognitoUserCredential CreateInstance(int userIndex)
	{
		constructUserIndex = userIndex;
		return new AWSCognitoUserCredential();
	}
	
	private AWSCognitoUserCredential() : base(Config.Find<AwsConfig>().cognitoID, RegionEndpoint.USEast1)
	{
	}
	
	private static KVStore _kvStore;
	private static object _lock = new object();
	
	private static KVStore KvStore
	{
		get
		{
			lock (_lock)
			{
				if (_kvStore == null)
					_kvStore = new PlayerPreferenceKVStore();
				
				return _kvStore;
			}
		}
	}
	
	private string _cacheKey;
	private string CACHE_KEY 
	{ 
		get 
		{
			if(string.IsNullOrEmpty(_cacheKey))
				_cacheKey = GetCacheKey(UserIndex);
			return _cacheKey;
		}
	}
	
	public override void ClearIdentityCache ()
	{
		KvStore.Clear(CACHE_KEY);
	}
	
	public override void CacheIdentityId (string identityId)
	{
		KvStore.Put(CACHE_KEY, identityId);
	}
	
	public override string GetCachedIdentityId ()
	{
		return KvStore.Get(CACHE_KEY);
	}
	
	private string GetCacheKey(int userIndex)
	{
		return "USERID" + userIndex;
	}
}

#endif