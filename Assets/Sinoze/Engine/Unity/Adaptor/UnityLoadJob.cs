#if UNITY_5
using UnityEngine;
using System.Collections;
using Sinoze.Engine;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


public class UnityLoadJob : LoadJob<WWW>
{
	public UnityLoadJob(LoadInstruction instruction)
		: base(instruction)
	{
	}

	#if UNITY_EDITOR
	public static bool Validator (object sender, X509Certificate certificate, X509Chain chain,
	                              SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}
	#endif

	protected override void LoadStart ()
	{
		#if UNITY_EDITOR
		ServicePointManager.ServerCertificateValidationCallback = Validator;
		#endif
		WebClient wc = new WebClient();
		wc.OpenRead(Instruction.url);
		int bytes_total = int.Parse(wc.ResponseHeaders["Content-Length"]);


		//
		//FIX 
		base.Asset = new WWW(Instruction.url);
		//base.Asset = WWW.LoadFromCacheOrDownload(Instruction.url, 0);

		base.priority = Instruction.priority;
		base.FileSize = (int)bytes_total;
	}

	public override void LoadUpdate ()
	{
		if(Asset != null)
		{
			if(Asset.isDone)
			{
				if(string.IsNullOrEmpty(Asset.error))
				{
					if(Asset.assetBundle != null)
					{
						// TODO: implement async
						Asset.assetBundle.LoadAllAssets();
					}
					Progress = 1;
					IsDoneSuccess = true;
				}
				else
				{
					Logger.Log(Asset.error);
					IsDoneFailure = true;
					Dispose();
				}
			}
			else
			{
				Progress = Asset.progress;
			}
		}
	}
}
#endif