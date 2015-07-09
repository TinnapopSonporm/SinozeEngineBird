

using System;

public class SinozeUserDataSet
{
	public string Name { get; private set; }
	
	private SinozeUserDataSetCollection _collection;
	
	private string UserID { get { return _collection.Owner.Identity; } }
	
	SinozeUserData userDataModule;
	
	public SinozeUserDataSet(SinozeUserDataSetCollection collection, string name)
	{
		this.Name = name;
		this._collection = collection;
		this.userDataModule = Module.Find<SinozeUserData>();
	}
	
	public string Get(string key)
	{
		return userDataModule.Get(_collection.Owner, Name, key);
	}
	
	public void Put(string key, string value)
	{
		userDataModule.Put(_collection.Owner, Name, key, value);
	}
	
	public void Sync(Action<bool> result)
	{
		userDataModule.Sync(_collection.Owner, Name, result);
	}
}