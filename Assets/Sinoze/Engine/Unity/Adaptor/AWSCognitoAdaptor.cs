#if AWS
using UnityEngine;
using System.Collections;
using Sinoze.Engine;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using System.Collections.Generic;
using System;
using Amazon.CognitoIdentity.Model;
using Amazon;
using Amazon.Util.Storage;
using Amazon.Util.Storage.Internal;

[Module(Service = typeof(SinozeUserIdentity))]
public class AWSCognitoAdaptor
{
	Dictionary<int, AWSCognitoUserCredential> credentials;
	Dictionary<string, int> _userIndexLookUp;

	public AWSCognitoAdaptor()
	{
		credentials = new Dictionary<int, AWSCognitoUserCredential>();
		_userIndexLookUp = new Dictionary<string, int>();
	}

	public AWSCognitoUserCredential GetCredential(int userIndex)
	{
		AWSCognitoUserCredential credential;
		if(!credentials.TryGetValue(userIndex, out credential))
		{
			credential = AWSCognitoUserCredential.CreateInstance(userIndex);
			credentials.Add(userIndex, credential);

			var cachedIdentity = credential.GetCachedIdentityId();
			if(!string.IsNullOrEmpty(cachedIdentity))
				_userIndexLookUp[cachedIdentity] = userIndex;
		}
		return credential;
	}

	public AWSCognitoUserCredential GetCredential(string userId)
	{
		var credential = GetCredential(_userIndexLookUp[userId]);
		if(credential == null || credential.GetIdentityId() != userId)
			throw new InvalidOperationException();
		return credential;
	}

	// adaptor method
	public void GetIdAsync(int userIndex, Dictionary<ThirdPartyLogin, string> thirdPartyLogins, Action<string> idResult)
	{
		var credential = GetCredential(userIndex);

		if(thirdPartyLogins != null)
		{
			foreach(var t in thirdPartyLogins)
			{
				credential.AddLogin(ThirdPartyId(t.Key), t.Value);
			}
		}

		credential.GetIdentityIdAsync((r) =>
		{
			if(!string.IsNullOrEmpty(r.Response))
				_userIndexLookUp[r.Response] = userIndex;
			idResult(r.Response);
		});
	}

	private string ThirdPartyId(ThirdPartyLogin thirdPartyLogin)
	{
		switch(thirdPartyLogin)
		{
			case ThirdPartyLogin.Facebook : return "graph.facebook.com";
			default : return null;
		}
	}
}
#endif