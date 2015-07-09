using UnityEngine;
using System.Collections;
using Sinoze.Engine;

//[Module]
public class TestUser : MonoBehaviour 
{
	void Start () 
	{
		Debug.Log("start getting user");
		SinozeUserIdentity.GetOrCreateUserAsync(0, (user) =>
		{
			Debug.Log("first id : " + user.Identity);

			user.AddThirdPartyLogin(ThirdPartyLogin.Facebook, "CAAUlikaAMLQBAOP5vixvRaJIDCzzEsZBUjOVPrQPBB667rw3F5OMjPZCayj6NL7SsKGu0bWbfUZAs8toNHdZBCMLvr2DJpWg2E5G5E9p0MOZC459qCF80KQummKCosuNxBXiuVqpf7sDAxTNZBkMac81CtMQPHXTc61U8XAjT8A0TO0RkQVCFWlcd1GRJEEbXZB4JU5UHvTwxPIu463fpB6",
			(r)=>
			{
				Debug.Log("facebook linked : " + r.Identity);
			});

		});
	}
}
