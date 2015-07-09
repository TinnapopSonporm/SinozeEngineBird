using System;
using System.Collections.Generic;
using Sinoze.Engine;

public class SinozeUser
{
	public string Identity { get; private set; }
	public int Index { get; private set; }
	public Dictionary<ThirdPartyLogin, string> ThirdPartyLogins { get; private set; }
	public SinozeUserDataSetCollection DataSets { get; private set; }
	
	public event Action<SinozeUser> RemovedOrChanged;
	public bool IsRemoved { get; private set; }
	
	public SinozeUser(int index, string userId, Dictionary<ThirdPartyLogin, string> thirdPartyLogins = null)
	{
		this.Index = index;
		this.Identity = userId;
		if(thirdPartyLogins != null)
			this.ThirdPartyLogins = new Dictionary<ThirdPartyLogin, string>(thirdPartyLogins);
		this.DataSets = new SinozeUserDataSetCollection(this);

		SinozeUserIdentity.UserCreatedOrChanged += HandleUserCreatedOrChanged;
	}
	
	public void AddThirdPartyLogin(ThirdPartyLogin login, string accessKey, Action<SinozeUser> result)
	{
		if(ThirdPartyLogins == null)
			ThirdPartyLogins = new Dictionary<ThirdPartyLogin, string>();
		ThirdPartyLogins.Add(login, accessKey);

		Module.Find<SinozeUserIdentity>().Resync(this, result); 
	}
	
	void HandleUserCreatedOrChanged (SinozeUser oldUser, SinozeUser newUser)
	{
		if(oldUser == this && newUser != this)
		{
			_RemovedOrChanged (newUser);
		}
	}

	private void _RemovedOrChanged(SinozeUser newUser)
	{
		IsRemoved = true;
		if(RemovedOrChanged != null)
		{
			RemovedOrChanged(newUser);
			RemovedOrChanged = null;
		}
	}
	
	public override string ToString ()
	{
		return string.Format ("[SinozeUserIdentity: UserID={0}, UserIndex={1}, ThirdPartyLogins={2}]", Identity, Index, ThirdPartyLogins);
	}
}